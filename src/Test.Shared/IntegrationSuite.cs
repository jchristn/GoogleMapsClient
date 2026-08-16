using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GoogleMapsClient;
using Touchstone.Core;

namespace Test.Shared
{
    /// <summary>
    /// Integration coverage for the client's HTTP paths. Because
    /// <see cref="GoogleMaps.BaseUrl"/> is configurable, the geocoding methods can be pointed at a
    /// local <see cref="MockHttpServer"/> that returns canned Google payloads. This exercises URL
    /// construction, the HTTP round-trip, the library's internal deserialization, and the mapping
    /// into <see cref="GoogleMapsAddress"/> — all without a real API key or internet access.
    ///
    /// Note: the Time Zone endpoint URL is hard-coded in the library and therefore cannot be
    /// redirected to the mock, so it is not integration-tested here; its parsing is covered by the
    /// deserialization tests in <see cref="ModelSuites"/>.
    /// </summary>
    internal static class IntegrationSuite
    {
        private const double Tol = 1e-6;

        internal static TestSuiteDescriptor GeocodeIntegrationSuite()
        {
            const string suite = "Integration.Geocode";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMaps - Geocode over Mock HTTP",
                cases: new List<TestCaseDescriptor>
                {
                    new TestCaseDescriptor(suite, "QueryCoordinatesReturnsParsedAddress",
                        "QueryCoordinatesAsync round-trips a mock response into a parsed address",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer(Fixtures.GeocodeOk))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                GoogleMapsAddress addr = await client.QueryCoordinatesAsync(37.42, -122.08, ct);

                                Verify.IsNotNull(addr);
                                Verify.AreEqual("Mountain View", addr.City);
                                Verify.IsNotNull(addr.Latitude);
                                Verify.AreEqual(37.4220656, addr.Latitude.Value, Tol);
                                Verify.AreEqual(1, server.RequestCount);
                                Verify.IsTrue(server.LastRequestLine.Contains("latlng="),
                                    "Request should carry a latlng parameter. Actual: " + server.LastRequestLine);
                            }
                        }),

                    new TestCaseDescriptor(suite, "QueryAddressReturnsParsedAddress",
                        "QueryAddressAsync round-trips a mock response into a parsed address",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer(Fixtures.GeocodeOk))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                GoogleMapsAddress addr = await client.QueryAddressAsync("1600 Amphitheatre Parkway", ct);

                                Verify.IsNotNull(addr);
                                Verify.AreEqual("California", addr.State);
                                Verify.AreEqual("CA", addr.StateAbbreviated);
                                Verify.IsTrue(server.LastRequestLine.Contains("address="),
                                    "Request should carry an address parameter. Actual: " + server.LastRequestLine);
                            }
                        }),

                    new TestCaseDescriptor(suite, "RequestUrlContainsApiKey",
                        "The outgoing request URL carries the configured API key",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer(Fixtures.GeocodeOk))
                            using (GoogleMaps client = new GoogleMaps("test-key-123") { BaseUrl = server.BaseUrl })
                            {
                                await client.QueryCoordinatesAsync(1.0, 2.0, ct);
                                Verify.IsTrue(server.LastRequestLine.Contains("key=test-key-123"),
                                    "Request should carry the API key. Actual: " + server.LastRequestLine);
                            }
                        }),

                    new TestCaseDescriptor(suite, "ZeroResultsProducesEmptyAddress",
                        "A ZERO_RESULTS response yields an address with the status but no populated fields",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer(Fixtures.GeocodeZeroResults))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                GoogleMapsAddress addr = await client.QueryCoordinatesAsync(0.0, 0.0, ct);

                                Verify.IsNotNull(addr);
                                Verify.IsNotNull(addr.GoogleResponse);
                                Verify.AreEqual("ZERO_RESULTS", addr.GoogleResponse.Status);
                                Verify.IsNull(addr.City);
                                Verify.IsNull(addr.Latitude);
                            }
                        }),

                    new TestCaseDescriptor(suite, "EmptyBodyReturnsNull",
                        "An empty response body causes the query to return null",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer(string.Empty))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                GoogleMapsAddress addr = await client.QueryCoordinatesAsync(1.0, 2.0, ct);
                                Verify.IsNull(addr);
                            }
                        }),

                    new TestCaseDescriptor(suite, "LoggerInvokedDuringRequest",
                        "The Logger callback is invoked while performing a request",
                        async ct =>
                        {
                            List<string> logs = new List<string>();
                            using (MockHttpServer server = new MockHttpServer(Fixtures.GeocodeOk))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                client.Logger = msg => { lock (logs) { logs.Add(msg); } };
                                await client.QueryCoordinatesAsync(1.0, 2.0, ct);
                            }

                            Verify.IsTrue(logs.Count > 0, "Expected the logger to be invoked at least once.");
                        }),

                    new TestCaseDescriptor(suite, "AddressQueryUrlEncodesSpecialCharacters",
                        "QueryAddressAsync URL-encodes reserved characters in the address before sending",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer(Fixtures.GeocodeOk))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                await client.QueryAddressAsync("A & B Street, Zürich", ct);

                                // The raw ampersand must not leak into the query string (it would be parsed
                                // as a parameter separator); it must appear percent-encoded instead.
                                Verify.IsTrue(server.LastRequestLine.Contains("%26"),
                                    "Ampersand should be percent-encoded to %26. Actual: " + server.LastRequestLine);
                                Verify.IsFalse(server.LastRequestLine.Contains("A & B"),
                                    "The unencoded address should not appear in the request line. Actual: " + server.LastRequestLine);
                            }
                        }),

                    new TestCaseDescriptor(suite, "MalformedJsonBodyThrows",
                        "A malformed JSON response body surfaces as a JsonException rather than being swallowed",
                        async ct =>
                        {
                            using (MockHttpServer server = new MockHttpServer("{ this is not valid json "))
                            using (GoogleMaps client = new GoogleMaps("k") { BaseUrl = server.BaseUrl })
                            {
                                await Verify.ThrowsAsync<System.Text.Json.JsonException>(
                                    () => client.QueryCoordinatesAsync(1.0, 2.0, ct));
                            }
                        }),

                    new TestCaseDescriptor(suite, "QueryCoordinatesHonorsCancelledToken",
                        "QueryCoordinatesAsync throws when handed an already-cancelled token",
                        async ct =>
                        {
                            using (GoogleMaps client = new GoogleMaps("k"))
                            using (System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource())
                            {
                                cts.Cancel();
                                await Verify.ThrowsAsync<OperationCanceledException>(
                                    () => client.QueryCoordinatesAsync(1.0, 2.0, cts.Token));
                            }
                        }),

                    new TestCaseDescriptor(suite, "QueryAddressHonorsCancelledToken",
                        "QueryAddressAsync throws when handed an already-cancelled token",
                        async ct =>
                        {
                            using (GoogleMaps client = new GoogleMaps("k"))
                            using (System.Threading.CancellationTokenSource cts = new System.Threading.CancellationTokenSource())
                            {
                                cts.Cancel();
                                await Verify.ThrowsAsync<OperationCanceledException>(
                                    () => client.QueryAddressAsync("1600 Amphitheatre Parkway", cts.Token));
                            }
                        }),
                });
        }
    }
}
