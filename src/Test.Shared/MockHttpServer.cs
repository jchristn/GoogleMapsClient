using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Shared
{
    /// <summary>
    /// A tiny loopback HTTP/1.1 server used to exercise the network paths of
    /// <see cref="GoogleMapsClient.GoogleMaps"/> without contacting Google or requiring a real API key.
    ///
    /// Built on <see cref="TcpListener"/> bound to 127.0.0.1:0 so it needs no elevated
    /// privileges or URL ACL reservations, which makes it safe and deterministic in CI.
    /// It reads the request headers, records them for assertions, and replies with a
    /// configurable status code and body, closing the connection after each response.
    /// </summary>
    internal sealed class MockHttpServer : IDisposable
    {
        private readonly TcpListener _Listener;
        private readonly int _StatusCode;
        private readonly byte[] _BodyBytes;
        private readonly string _ContentType;
        private readonly CancellationTokenSource _Cts;
        private int _RequestCount;
        private string _LastRequestLine;

        /// <summary>The ephemeral port the server is listening on.</summary>
        internal int Port { get; }

        /// <summary>
        /// A base URL suitable for <see cref="GoogleMapsClient.GoogleMaps.BaseUrl"/>. The client
        /// appends the API key and query string to this value, mirroring the real endpoint shape.
        /// </summary>
        internal string BaseUrl
        {
            get { return "http://127.0.0.1:" + Port + "/maps/api/geocode/json?sensor=false&key="; }
        }

        /// <summary>The number of requests the server has fully received.</summary>
        internal int RequestCount { get { return Volatile.Read(ref _RequestCount); } }

        /// <summary>The request line (e.g. "GET /... HTTP/1.1") of the most recent request.</summary>
        internal string LastRequestLine { get { return Volatile.Read(ref _LastRequestLine); } }

        internal MockHttpServer(string body, int statusCode = 200, string contentType = "application/json")
        {
            _BodyBytes = Encoding.UTF8.GetBytes(body ?? string.Empty);
            _StatusCode = statusCode;
            _ContentType = contentType;

            _Listener = new TcpListener(IPAddress.Loopback, 0);
            _Listener.Start();
            Port = ((IPEndPoint)_Listener.LocalEndpoint).Port;

            _Cts = new CancellationTokenSource();
            _ = Task.Run(() => AcceptLoopAsync(_Cts.Token));
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client;

                try
                {
                    client = await _Listener.AcceptTcpClientAsync().ConfigureAwait(false);
                }
                catch
                {
                    break; // listener stopped / disposed
                }

                _ = Task.Run(() => HandleClientAsync(client));
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[4096];
                    StringBuilder request = new StringBuilder();
                    int read;

                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                    {
                        request.Append(Encoding.ASCII.GetString(buffer, 0, read));
                        if (request.ToString().Contains("\r\n\r\n")) break; // end of request headers
                    }

                    string requestText = request.ToString();
                    int newline = requestText.IndexOf('\n');
                    string firstLine = newline >= 0 ? requestText.Substring(0, newline).TrimEnd('\r') : requestText;
                    Volatile.Write(ref _LastRequestLine, firstLine);
                    Interlocked.Increment(ref _RequestCount);

                    StringBuilder header = new StringBuilder();
                    header.Append("HTTP/1.1 " + _StatusCode + " " + ReasonPhrase(_StatusCode) + "\r\n");
                    header.Append("Content-Type: " + _ContentType + "\r\n");
                    header.Append("Content-Length: " + _BodyBytes.Length + "\r\n");
                    header.Append("Connection: close\r\n");
                    header.Append("\r\n");

                    byte[] headerBytes = Encoding.ASCII.GetBytes(header.ToString());
                    await stream.WriteAsync(headerBytes, 0, headerBytes.Length).ConfigureAwait(false);

                    if (_BodyBytes.Length > 0)
                        await stream.WriteAsync(_BodyBytes, 0, _BodyBytes.Length).ConfigureAwait(false);

                    await stream.FlushAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                // Best-effort mock: ignore per-connection I/O failures.
            }
        }

        private static string ReasonPhrase(int statusCode)
        {
            switch (statusCode)
            {
                case 200: return "OK";
                case 400: return "Bad Request";
                case 404: return "Not Found";
                case 500: return "Internal Server Error";
                case 503: return "Service Unavailable";
                default: return "Status";
            }
        }

        public void Dispose()
        {
            try { _Cts.Cancel(); } catch { }
            try { _Listener.Stop(); } catch { }
            try { _Cts.Dispose(); } catch { }
        }
    }
}
