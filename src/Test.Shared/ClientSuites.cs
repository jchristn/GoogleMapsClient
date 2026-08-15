using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GoogleMapsClient;
using Touchstone.Core;

namespace Test.Shared
{
    /// <summary>
    /// Suites covering the <see cref="GoogleMaps"/> client surface that does not require network
    /// access: construction, property getters/setters, argument validation, and disposal semantics.
    /// All of these paths either short-circuit before any HTTP call or fail fast, so they are fully
    /// hermetic and deterministic.
    /// </summary>
    internal static class ClientSuites
    {
        private const string DefaultBaseUrl = "https://maps.googleapis.com/maps/api/geocode/json?sensor=false&key=";

        private static TestCaseDescriptor Case(string suiteId, string caseId, string displayName, Action body)
        {
            return new TestCaseDescriptor(
                suiteId: suiteId,
                caseId: caseId,
                displayName: displayName,
                executeAsync: ct => { body(); return Task.CompletedTask; });
        }

        // ------------------------------------------------------------------ Construction

        internal static TestSuiteDescriptor ConstructionSuite()
        {
            const string suite = "Client.Construction";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMaps - Construction",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "NullApiKeyThrows", "Constructor rejects a null API key", () =>
                        Verify.Throws<ArgumentNullException>(() => new GoogleMaps(null))),

                    Case(suite, "EmptyApiKeyThrows", "Constructor rejects an empty API key", () =>
                        Verify.Throws<ArgumentNullException>(() => new GoogleMaps(string.Empty))),

                    Case(suite, "ValidApiKeyStored", "Constructor stores a valid API key", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("my-secret-key"))
                            Verify.AreEqual("my-secret-key", client.ApiKey);
                    }),

                    Case(suite, "WhitespaceApiKeyAccepted", "Constructor accepts a whitespace key (only null/empty are rejected)", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps(" "))
                            Verify.AreEqual(" ", client.ApiKey);
                    }),
                });
        }

        // ------------------------------------------------------------------ Properties

        internal static TestSuiteDescriptor PropertySuite()
        {
            const string suite = "Client.Properties";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMaps - Properties",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "DefaultBaseUrl", "BaseUrl defaults to the Google geocode endpoint", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.AreEqual(DefaultBaseUrl, client.BaseUrl);
                    }),

                    Case(suite, "SetBaseUrl", "BaseUrl accepts a custom value", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                        {
                            client.BaseUrl = "http://localhost:9999/geo?key=";
                            Verify.AreEqual("http://localhost:9999/geo?key=", client.BaseUrl);
                        }
                    }),

                    Case(suite, "SetNullBaseUrlThrows", "BaseUrl rejects null", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.Throws<ArgumentNullException>(() => client.BaseUrl = null);
                    }),

                    Case(suite, "SetEmptyBaseUrlThrows", "BaseUrl rejects empty string", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.Throws<ArgumentNullException>(() => client.BaseUrl = string.Empty);
                    }),

                    Case(suite, "DefaultTimeout", "TimeoutMs defaults to 15000", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.AreEqual(15000, client.TimeoutMs);
                    }),

                    Case(suite, "SetTimeout", "TimeoutMs accepts a positive value", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                        {
                            client.TimeoutMs = 5000;
                            Verify.AreEqual(5000, client.TimeoutMs);
                        }
                    }),

                    Case(suite, "SetTimeoutMinBoundary", "TimeoutMs accepts the minimum legal value of 1", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                        {
                            client.TimeoutMs = 1;
                            Verify.AreEqual(1, client.TimeoutMs);
                        }
                    }),

                    Case(suite, "SetZeroTimeoutThrows", "TimeoutMs rejects zero", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.Throws<ArgumentOutOfRangeException>(() => client.TimeoutMs = 0);
                    }),

                    Case(suite, "SetNegativeTimeoutThrows", "TimeoutMs rejects a negative value", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.Throws<ArgumentOutOfRangeException>(() => client.TimeoutMs = -100);
                    }),

                    Case(suite, "DefaultLoggerNull", "Logger defaults to null", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                            Verify.IsNull(client.Logger);
                    }),

                    Case(suite, "SetLogger", "Logger can be assigned", () =>
                    {
                        using (GoogleMaps client = new GoogleMaps("k"))
                        {
                            client.Logger = _ => { };
                            Verify.IsNotNull(client.Logger);
                        }
                    }),
                });
        }

        // ------------------------------------------------------------------ Argument validation

        internal static TestSuiteDescriptor ArgumentValidationSuite()
        {
            const string suite = "Client.ArgumentValidation";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMaps - Argument Validation",
                cases: new List<TestCaseDescriptor>
                {
                    new TestCaseDescriptor(suite, "QueryAddressNullThrows",
                        "QueryAddressAsync rejects a null address",
                        async ct =>
                        {
                            using (GoogleMaps client = new GoogleMaps("k"))
                                await Verify.ThrowsAsync<ArgumentNullException>(() => client.QueryAddressAsync(null, ct));
                        }),

                    new TestCaseDescriptor(suite, "QueryAddressEmptyThrows",
                        "QueryAddressAsync rejects an empty address",
                        async ct =>
                        {
                            using (GoogleMaps client = new GoogleMaps("k"))
                                await Verify.ThrowsAsync<ArgumentNullException>(() => client.QueryAddressAsync(string.Empty, ct));
                        }),

                    new TestCaseDescriptor(suite, "LocalTimestampAddressNullThrows",
                        "LocalTimestampAsync(address) rejects a null address",
                        async ct =>
                        {
                            using (GoogleMaps client = new GoogleMaps("k"))
                                await Verify.ThrowsAsync<ArgumentNullException>(() => client.LocalTimestampAsync((string)null, DateTime.UtcNow, ct));
                        }),

                    new TestCaseDescriptor(suite, "LocalTimestampAddressEmptyThrows",
                        "LocalTimestampAsync(address) rejects an empty address",
                        async ct =>
                        {
                            using (GoogleMaps client = new GoogleMaps("k"))
                                await Verify.ThrowsAsync<ArgumentNullException>(() => client.LocalTimestampAsync(string.Empty, DateTime.UtcNow, ct));
                        }),
                });
        }

        // ------------------------------------------------------------------ Disposal

        internal static TestSuiteDescriptor DisposalSuite()
        {
            const string suite = "Client.Disposal";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMaps - Disposal",
                cases: new List<TestCaseDescriptor>
                {
                    new TestCaseDescriptor(suite, "QueryCoordinatesAfterDisposeThrows",
                        "QueryCoordinatesAsync throws ObjectDisposedException after Dispose",
                        async ct =>
                        {
                            GoogleMaps client = new GoogleMaps("k");
                            client.Dispose();
                            await Verify.ThrowsAsync<ObjectDisposedException>(() => client.QueryCoordinatesAsync(1.0, 2.0, ct));
                        }),

                    new TestCaseDescriptor(suite, "QueryAddressAfterDisposeThrowsBeforeNullCheck",
                        "QueryAddressAsync checks disposal before validating the address argument",
                        async ct =>
                        {
                            GoogleMaps client = new GoogleMaps("k");
                            client.Dispose();
                            // Even a null address yields ObjectDisposedException because the disposal guard runs first.
                            await Verify.ThrowsAsync<ObjectDisposedException>(() => client.QueryAddressAsync(null, ct));
                        }),

                    new TestCaseDescriptor(suite, "LocalTimestampCoordsAfterDisposeThrows",
                        "LocalTimestampAsync(coordinates) throws ObjectDisposedException after Dispose",
                        async ct =>
                        {
                            GoogleMaps client = new GoogleMaps("k");
                            client.Dispose();
                            await Verify.ThrowsAsync<ObjectDisposedException>(() => client.LocalTimestampAsync(1.0, 2.0, DateTime.UtcNow, ct));
                        }),

                    new TestCaseDescriptor(suite, "LocalTimestampAddressAfterDisposeThrows",
                        "LocalTimestampAsync(address) throws ObjectDisposedException after Dispose",
                        async ct =>
                        {
                            GoogleMaps client = new GoogleMaps("k");
                            client.Dispose();
                            await Verify.ThrowsAsync<ObjectDisposedException>(() => client.LocalTimestampAsync("somewhere", DateTime.UtcNow, ct));
                        }),

                    Case(suite, "DisposeIsIdempotent", "Dispose can be called multiple times safely", () =>
                    {
                        GoogleMaps client = new GoogleMaps("k");
                        client.Dispose();
                        client.Dispose(); // must not throw
                    }),

                    Case(suite, "ApiKeyReadableAfterDispose", "ApiKey remains readable after Dispose", () =>
                    {
                        GoogleMaps client = new GoogleMaps("readable-key");
                        client.Dispose();
                        Verify.AreEqual("readable-key", client.ApiKey);
                    }),
                });
        }
    }
}
