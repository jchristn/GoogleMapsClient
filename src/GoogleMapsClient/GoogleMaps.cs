using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestWrapper;

namespace GoogleMapsClient
{
    /// <summary>
    /// Google Maps client.
    /// </summary>
    public class GoogleMaps
    {
        #region Public-Members

        /// <summary>
        /// Google Maps API key.
        /// </summary>
        public string ApiKey
        {
            get
            {
                return _ApiKey;
            }
        }

        /// <summary>
        /// Base URL for RESTful API requests. 
        /// Do not modify unless you are emulating a Google Maps API server.
        /// </summary>
        public string BaseUrl
        {
            get
            {
                return _BaseUrl;
            }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(BaseUrl));
                _BaseUrl = value;
            }
        }

        /// <summary>
        /// RESTful timeout in milliseconds.
        /// </summary>
        public int TimeoutMs
        {
            get
            {
                return _TimeoutMs;
            }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException(nameof(TimeoutMs));
                _TimeoutMs = value;
            }
        }

        /// <summary>
        /// Method to use when sending log messages.
        /// </summary>
        public Action<string> Logger { get; set; } = null;

        #endregion

        #region Private-Members

        private string _ApiKey = null;
        private string _BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json?sensor=false&key=";
        private string _Header = "[GoogleMaps] ";
        private int _TimeoutMs = 15000;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="apiKey">Google Maps API key.</param>
        public GoogleMaps(string apiKey)
        {
            if (String.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));

            _ApiKey = apiKey;
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Retrieve address details for a specified set of coordinates.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <returns>Address details.</returns>
        public GoogleMapsAddress QueryCoordinates(double latitude, double longitude)
        {
            return QueryCoordinatesAsync(latitude, longitude).Result;
        }

        /// <summary>
        /// Retrieve address details for a specified set of coordinates.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Address details.</returns>
        public async Task<GoogleMapsAddress> QueryCoordinatesAsync(double latitude, double longitude, CancellationToken token = default)
        {
            string url = _BaseUrl + _ApiKey + "&latlng=" + latitude + "," + longitude;
            GoogleMapsResponse resp = await GetGoogleMapsResponseAsync(HttpMethod.Get, url, null, TimeoutMs, token).ConfigureAwait(false);
            return new GoogleMapsAddress(resp);
        }

        /// <summary>
        /// Retrieve address details for a specified address.
        /// </summary>
        /// <param name="address">Address.</param>
        /// <returns>Address details.</returns>
        public GoogleMapsAddress QueryAddress(string address)
        {
            return QueryAddressAsync(address).Result;
        }

        /// <summary>
        /// Retrieve address details for a specified address.
        /// </summary>
        /// <param name="address">Address.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Address details.</returns>
        public async Task<GoogleMapsAddress> QueryAddressAsync(string address, CancellationToken token = default)
        {
            if (String.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            string url = _BaseUrl + _ApiKey + "&address=" + WebUtility.UrlEncode(address);
            GoogleMapsResponse resp = await GetGoogleMapsResponseAsync(HttpMethod.Get, url, null, TimeoutMs, token).ConfigureAwait(false);
            return new GoogleMapsAddress(resp);
        }

        /// <summary>
        /// Generate a timestamp for a given set of coordinates.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <param name="timestamp">Timestamp for which the local timestamp should be retrieved.</param>
        /// <param name="timezone">Timezone string.</param>
        /// <returns>Local timestamp.</returns>
        public GoogleMapsTimestamp LocalTimestamp(double latitude, double longitude, DateTime timestamp)
        {
            return LocalTimestampAsync(latitude, longitude, timestamp).Result;
        }

        /// <summary>
        /// Generate a timestamp for a given set of coordinates.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <param name="timestamp">Timestamp for which the local timestamp should be retrieved.</param>
        /// <param name="timezone">Timezone string.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Local timestamp.</returns>
        public async Task<GoogleMapsTimestamp> LocalTimestampAsync(double latitude, double longitude, DateTime timestamp, CancellationToken token = default)
        {
            timestamp = timestamp.ToUniversalTime();
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = timestamp.ToUniversalTime() - origin;
            double ts = Math.Floor(diff.TotalSeconds);

            string url = "https://maps.googleapis.com/maps/api/timezone/json?location=" + latitude + "," + longitude + "&key=" + _ApiKey + "&timestamp=" + ts;
            string result = await GetRestResponseAsync(HttpMethod.Get, url, null, TimeoutMs, token).ConfigureAwait(false);
            return SerializationHelper.DeserializeJson<GoogleMapsTimestamp>(result);
        }

        /// <summary>
        /// Retrieve the local timestamp for a given address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="timestamp">Timestamp for which the local timestamp should be retrieved.</param>
        /// <param name="timezone">Timezone string.</param>
        /// <returns>Local timestamp.</returns>
        public GoogleMapsTimestamp LocalTimestamp(string address, DateTime timestamp)
        {
            return LocalTimestampAsync(address, timestamp).Result;
        }

        /// <summary>
        /// Retrieve the local timestamp for a given address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="timestamp">Timestamp for which the local timestamp should be retrieved.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Local timestamp.</returns>
        public async Task<GoogleMapsTimestamp> LocalTimestampAsync(string address, DateTime timestamp, CancellationToken token = default)
        {
            if (String.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            GoogleMapsAddress addr = await QueryAddressAsync(address, token).ConfigureAwait(false);
            return await LocalTimestampAsync(
                Convert.ToDouble(addr.Latitude),
                Convert.ToDouble(addr.Longitude),
                timestamp,
                token).ConfigureAwait(false);
        }

        #endregion

        #region Private-Methods

        private async Task<GoogleMapsResponse> GetGoogleMapsResponseAsync(HttpMethod method, string url, string body = null, int timeoutMs = 15000, CancellationToken token = default)
        {
            return SerializationHelper.DeserializeJson<GoogleMapsResponse>(await GetRestResponseAsync(method, url, body, timeoutMs, token).ConfigureAwait(false));
        }

        private async Task<string> GetRestResponseAsync(HttpMethod method, string url, string body = null, int timeoutMs = 15000, CancellationToken token = default)
        {
            using (RestRequest req = new RestRequest(url, method))
            {
                req.TimeoutMilliseconds = timeoutMs;

                if (!String.IsNullOrEmpty(body))
                {
                    using (RestResponse resp = await req.SendAsync(body, token).ConfigureAwait(false))
                    {
                        if (resp != null)
                        {
                            string data = resp.DataAsString;
                            Logger?.Invoke(_Header + "response:" + Environment.NewLine + resp.DataAsString);
                            return resp.DataAsString;
                        }
                        else
                        {
                            Logger?.Invoke(_Header + "null response");
                            return null;
                        }
                    }
                }
                else
                {
                    using (RestResponse resp = await req.SendAsync(token).ConfigureAwait(false))
                    {
                        if (resp != null)
                        {
                            string data = resp.DataAsString;
                            Logger?.Invoke(_Header + "response:" + Environment.NewLine + resp.DataAsString);
                            return resp.DataAsString;
                        }
                        else
                        {
                            Logger?.Invoke(_Header + "null response");
                            return null;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
