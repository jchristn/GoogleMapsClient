using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GoogleMapsClient;
using Touchstone.Core;

namespace Test.Shared
{
    /// <summary>
    /// Suites covering the plain data models: <see cref="GoogleMapsResponse"/> and its nested
    /// types, <see cref="GoogleMapsCoordinates"/>, and <see cref="GoogleMapsTimestamp"/> including
    /// its computed <see cref="GoogleMapsTimestamp.LocalTime"/> property, plus JSON round-tripping.
    /// </summary>
    internal static class ModelSuites
    {
        private const double Tol = 1e-6;

        private static TestCaseDescriptor Case(string suiteId, string caseId, string displayName, Action body)
        {
            return new TestCaseDescriptor(
                suiteId: suiteId,
                caseId: caseId,
                displayName: displayName,
                executeAsync: ct => { body(); return Task.CompletedTask; });
        }

        // ------------------------------------------------------------------ Response model

        internal static TestSuiteDescriptor ResponseModelSuite()
        {
            const string suite = "Model.Response";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMapsResponse - Model & Deserialization",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "Defaults", "A new response has an empty result list and null status", () =>
                    {
                        GoogleMapsResponse resp = new GoogleMapsResponse();
                        Verify.IsNotNull(resp.Results);
                        Verify.AreEqual(0, resp.Results.Count);
                        Verify.IsNull(resp.Status);
                    }),

                    Case(suite, "DeserializeFullResponse", "A full geocode payload deserializes into the model graph", () =>
                    {
                        GoogleMapsResponse resp = Json.Deserialize<GoogleMapsResponse>(Fixtures.GeocodeOk);
                        Verify.AreEqual("OK", resp.Status);
                        Verify.AreEqual(1, resp.Results.Count);

                        GoogleMapsResponse.Result r = resp.Results[0];
                        Verify.AreEqual("ChIJ2eUgeAK6j4ARbn5u_wAGqWA", r.PlaceId);
                        Verify.AreEqual(9, r.AddressComponents.Count);
                        Verify.AreEqual(1, r.Types.Count);
                        Verify.AreEqual("street_address", r.Types[0]);
                        Verify.IsNotNull(r.PartialMatch);
                        Verify.IsTrue(r.PartialMatch.Value);

                        Verify.IsNotNull(r.Geometry);
                        Verify.IsNotNull(r.Geometry.Location);
                        Verify.AreEqual(37.4220656, r.Geometry.Location.Latitude, Tol);
                        Verify.AreEqual(-122.0840897, r.Geometry.Location.Longitude, Tol);
                        Verify.AreEqual("ROOFTOP", r.Geometry.LocationType);
                        Verify.IsNotNull(r.Geometry.Bounds);
                        Verify.IsNotNull(r.Geometry.Bounds.Northeast);
                        Verify.IsNotNull(r.Geometry.Viewport);
                    }),

                    Case(suite, "DeserializeZeroResults", "A ZERO_RESULTS payload deserializes with an empty result list", () =>
                    {
                        GoogleMapsResponse resp = Json.Deserialize<GoogleMapsResponse>(Fixtures.GeocodeZeroResults);
                        Verify.AreEqual("ZERO_RESULTS", resp.Status);
                        Verify.AreEqual(0, resp.Results.Count);
                    }),

                    Case(suite, "DeserializeRequestDenied", "A REQUEST_DENIED payload deserializes with the denied status", () =>
                    {
                        GoogleMapsResponse resp = Json.Deserialize<GoogleMapsResponse>(Fixtures.GeocodeRequestDenied);
                        Verify.AreEqual("REQUEST_DENIED", resp.Status);
                        Verify.AreEqual(0, resp.Results.Count);
                    }),

                    Case(suite, "AddressComponentDefaults", "A new address component has an empty type list and null names", () =>
                    {
                        GoogleMapsResponse.Result.AddressComponent comp = new GoogleMapsResponse.Result.AddressComponent();
                        Verify.IsNotNull(comp.Types);
                        Verify.AreEqual(0, comp.Types.Count);
                        Verify.IsNull(comp.LongName);
                        Verify.IsNull(comp.ShortName);
                    }),
                });
        }

        // ------------------------------------------------------------------ Coordinates

        internal static TestSuiteDescriptor CoordinatesSuite()
        {
            const string suite = "Model.Coordinates";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMapsCoordinates - Model",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "Defaults", "Coordinates default to zero", () =>
                    {
                        GoogleMapsCoordinates c = new GoogleMapsCoordinates();
                        Verify.AreEqual(0.0, c.Latitude, Tol);
                        Verify.AreEqual(0.0, c.Longitude, Tol);
                    }),

                    Case(suite, "Settable", "Coordinates round-trip assigned values", () =>
                    {
                        GoogleMapsCoordinates c = new GoogleMapsCoordinates { Latitude = 12.34, Longitude = -56.78 };
                        Verify.AreEqual(12.34, c.Latitude, Tol);
                        Verify.AreEqual(-56.78, c.Longitude, Tol);
                    }),
                });
        }

        // ------------------------------------------------------------------ Timestamp

        internal static TestSuiteDescriptor TimestampSuite()
        {
            const string suite = "Model.Timestamp";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMapsTimestamp - Model & LocalTime",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "Defaults", "A new timestamp has the documented default values", () =>
                    {
                        GoogleMapsTimestamp ts = new GoogleMapsTimestamp();
                        Verify.AreEqual("OK", ts.Status);
                        Verify.AreEqual("America/Los_Angeles", ts.TimezoneId);
                        Verify.AreEqual("Pacific Daylight Time", ts.TimezoneName);
                        Verify.AreEqual(0.0, ts.DaylightSavingsTimeOffset, Tol);
                        Verify.AreEqual(0.0, ts.RawOffset, Tol);
                    }),

                    Case(suite, "DeserializeTimezone", "A time zone payload deserializes into the model", () =>
                    {
                        GoogleMapsTimestamp ts = Json.Deserialize<GoogleMapsTimestamp>(Fixtures.TimezoneOk);
                        Verify.AreEqual(3600.0, ts.DaylightSavingsTimeOffset, Tol);
                        Verify.AreEqual(-28800.0, ts.RawOffset, Tol);
                        Verify.AreEqual("OK", ts.Status);
                        Verify.AreEqual("America/Los_Angeles", ts.TimezoneId);
                        Verify.AreEqual("Pacific Daylight Time", ts.TimezoneName);
                    }),

                    Case(suite, "LocalTimeReflectsOffsets", "LocalTime equals UtcNow plus the DST and raw offsets", () =>
                    {
                        GoogleMapsTimestamp ts = new GoogleMapsTimestamp
                        {
                            DaylightSavingsTimeOffset = 3600,
                            RawOffset = -28800
                        };

                        double offsetSeconds = 3600 - 28800; // -25200 (PDT)
                        DateTime before = DateTime.UtcNow;
                        DateTime local = ts.LocalTime;
                        DateTime after = DateTime.UtcNow;

                        DateTime lower = before.AddSeconds(offsetSeconds).AddSeconds(-2);
                        DateTime upper = after.AddSeconds(offsetSeconds).AddSeconds(2);

                        Verify.IsTrue(local >= lower && local <= upper,
                            "LocalTime " + local.ToString("o") + " not within expected window ["
                            + lower.ToString("o") + ", " + upper.ToString("o") + "].");
                    }),

                    Case(suite, "LocalTimeZeroOffsetIsUtcNow", "With zero offsets LocalTime tracks UtcNow", () =>
                    {
                        GoogleMapsTimestamp ts = new GoogleMapsTimestamp
                        {
                            DaylightSavingsTimeOffset = 0,
                            RawOffset = 0
                        };

                        DateTime before = DateTime.UtcNow;
                        DateTime local = ts.LocalTime;
                        DateTime after = DateTime.UtcNow;

                        Verify.IsTrue(local >= before.AddSeconds(-2) && local <= after.AddSeconds(2),
                            "LocalTime with zero offset should approximately equal UtcNow.");
                    }),
                });
        }

        // ------------------------------------------------------------------ Serialization round-trips

        internal static TestSuiteDescriptor SerializationSuite()
        {
            const string suite = "Model.Serialization";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "Models - JSON Round-Trip",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "TimestampRoundTrip", "A timestamp survives a serialize/deserialize round-trip", () =>
                    {
                        GoogleMapsTimestamp original = new GoogleMapsTimestamp
                        {
                            DaylightSavingsTimeOffset = 7200,
                            RawOffset = 3600,
                            Status = "OK",
                            TimezoneId = "Europe/Berlin",
                            TimezoneName = "Central European Summer Time"
                        };

                        string json = Json.Serialize(original);
                        GoogleMapsTimestamp copy = Json.Deserialize<GoogleMapsTimestamp>(json);

                        Verify.AreEqual(7200.0, copy.DaylightSavingsTimeOffset, Tol);
                        Verify.AreEqual(3600.0, copy.RawOffset, Tol);
                        Verify.AreEqual("OK", copy.Status);
                        Verify.AreEqual("Europe/Berlin", copy.TimezoneId);
                        Verify.AreEqual("Central European Summer Time", copy.TimezoneName);
                    }),

                    Case(suite, "ResponseRoundTrip", "A response survives a serialize/deserialize round-trip", () =>
                    {
                        GoogleMapsResponse original = Json.Deserialize<GoogleMapsResponse>(Fixtures.GeocodeOk);
                        string json = Json.Serialize(original);
                        GoogleMapsResponse copy = Json.Deserialize<GoogleMapsResponse>(json);

                        Verify.AreEqual("OK", copy.Status);
                        Verify.AreEqual(1, copy.Results.Count);
                        Verify.AreEqual("ChIJ2eUgeAK6j4ARbn5u_wAGqWA", copy.Results[0].PlaceId);
                        Verify.AreEqual(9, copy.Results[0].AddressComponents.Count);
                        Verify.AreEqual(37.4220656, copy.Results[0].Geometry.Location.Latitude, Tol);
                    }),

                    Case(suite, "CoordinatesRoundTrip", "Coordinates survive a serialize/deserialize round-trip", () =>
                    {
                        GoogleMapsCoordinates original = new GoogleMapsCoordinates { Latitude = 40.7128, Longitude = -74.0060 };
                        string json = Json.Serialize(original);
                        GoogleMapsCoordinates copy = Json.Deserialize<GoogleMapsCoordinates>(json);

                        Verify.AreEqual(40.7128, copy.Latitude, Tol);
                        Verify.AreEqual(-74.0060, copy.Longitude, Tol);
                    }),

                    Case(suite, "AddressComponentRoundTrip", "An address component survives a serialize/deserialize round-trip", () =>
                    {
                        GoogleMapsResponse.Result.AddressComponent original = new GoogleMapsResponse.Result.AddressComponent
                        {
                            LongName = "California",
                            ShortName = "CA",
                            Types = new List<string> { "administrative_area_level_1", "political" }
                        };

                        string json = Json.Serialize(original);
                        GoogleMapsResponse.Result.AddressComponent copy =
                            Json.Deserialize<GoogleMapsResponse.Result.AddressComponent>(json);

                        Verify.AreEqual("California", copy.LongName);
                        Verify.AreEqual("CA", copy.ShortName);
                        Verify.AreEqual(2, copy.Types.Count);
                        Verify.AreEqual("political", copy.Types[1]);
                    }),
                });
        }
    }
}
