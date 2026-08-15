using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GoogleMapsClient;
using Touchstone.Core;

namespace Test.Shared
{
    using Result = GoogleMapsResponse.Result;
    using AddressComponent = GoogleMapsResponse.Result.AddressComponent;
    using GeometryComponent = GoogleMapsResponse.Result.GeometryComponent;
    using Boundary = GoogleMapsResponse.Result.GeometryComponent.Boundary;
    using GoogleCoordinates = GoogleMapsResponse.Result.GeometryComponent.GoogleCoordinates;

    /// <summary>
    /// Suite covering <see cref="GoogleMapsAddress"/>, which contains the bulk of the library's
    /// business logic: translating a raw <see cref="GoogleMapsResponse"/> into flattened,
    /// convenient address fields. Responses are built both by hand (to isolate individual branches)
    /// and by deserializing realistic JSON fixtures (to exercise the end-to-end pipeline).
    /// </summary>
    internal static class AddressSuite
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

        private static AddressComponent Comp(string longName, string shortName, params string[] types)
        {
            return new AddressComponent
            {
                LongName = longName,
                ShortName = shortName,
                Types = new List<string>(types)
            };
        }

        private static GoogleCoordinates Coord(double lat, double lng)
        {
            return new GoogleCoordinates { Latitude = lat, Longitude = lng };
        }

        internal static TestSuiteDescriptor AddressParsingSuite()
        {
            const string suite = "Address.Parsing";

            return new TestSuiteDescriptor(
                suiteId: suite,
                displayName: "GoogleMapsAddress - Parsing",
                cases: new List<TestCaseDescriptor>
                {
                    Case(suite, "NullResponseThrows", "Constructor rejects a null response", () =>
                        Verify.Throws<ArgumentNullException>(() => new GoogleMapsAddress(null))),

                    Case(suite, "DefaultConstructorAllNull", "Default constructor leaves all fields null", () =>
                    {
                        GoogleMapsAddress addr = new GoogleMapsAddress();
                        Verify.IsNull(addr.GoogleResponse);
                        Verify.IsNull(addr.FormattedAddress);
                        Verify.IsNull(addr.Latitude);
                        Verify.IsNull(addr.Longitude);
                        Verify.IsNull(addr.City);
                        Verify.IsNull(addr.State);
                        Verify.IsNull(addr.Country);
                        Verify.IsNull(addr.PartialMatch);
                        Verify.IsNull(addr.NortheastBoundary);
                        Verify.IsNull(addr.SouthwestBoundary);
                    }),

                    Case(suite, "NonOkStatusRetainsResponseButLeavesFieldsNull",
                        "A non-OK status stores the response but populates no convenience fields", () =>
                    {
                        GoogleMapsResponse resp = new GoogleMapsResponse
                        {
                            Status = "ZERO_RESULTS",
                            Results = new List<Result>()
                        };

                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);
                        Verify.IsNotNull(addr.GoogleResponse);
                        Verify.AreEqual("ZERO_RESULTS", addr.GoogleResponse.Status);
                        Verify.IsNull(addr.FormattedAddress);
                        Verify.IsNull(addr.Latitude);
                        Verify.IsNull(addr.City);
                        Verify.IsNull(addr.PartialMatch);
                    }),

                    Case(suite, "OkButEmptyResultsLeavesFieldsNull",
                        "An OK status with no results populates no convenience fields", () =>
                    {
                        GoogleMapsResponse resp = new GoogleMapsResponse
                        {
                            Status = "OK",
                            Results = new List<Result>()
                        };

                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);
                        Verify.IsNotNull(addr.GoogleResponse);
                        Verify.IsNull(addr.FormattedAddress);
                        Verify.IsNull(addr.Latitude);
                    }),

                    Case(suite, "FullResultParsedFromFixture",
                        "A complete OK response is flattened into every convenience field", () =>
                    {
                        GoogleMapsResponse resp = Json.Deserialize<GoogleMapsResponse>(Fixtures.GeocodeOk);
                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);

                        Verify.AreEqual("1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA", addr.FormattedAddress);
                        Verify.AreEqual("1600", addr.StreetNumber);
                        Verify.AreEqual("Amphitheatre Parkway", addr.StreetName);
                        Verify.AreEqual("Shoreline West", addr.Neighborhood);
                        Verify.AreEqual("Mountain View", addr.City);
                        Verify.AreEqual("Santa Clara County", addr.County);
                        Verify.AreEqual("California", addr.State);
                        Verify.AreEqual("CA", addr.StateAbbreviated);
                        Verify.AreEqual("United States", addr.Country);
                        Verify.AreEqual("US", addr.CountryAbbreviated);
                        Verify.AreEqual("94043", addr.Postal);
                        Verify.AreEqual("1351", addr.PostalSuffix);

                        Verify.IsNotNull(addr.Latitude);
                        Verify.IsNotNull(addr.Longitude);
                        Verify.AreEqual(37.4220656, addr.Latitude.Value, Tol);
                        Verify.AreEqual(-122.0840897, addr.Longitude.Value, Tol);

                        Verify.IsNotNull(addr.NortheastBoundary);
                        Verify.IsNotNull(addr.SouthwestBoundary);
                        Verify.AreEqual(37.4229909, addr.NortheastBoundary.Latitude, Tol);
                        Verify.AreEqual(-122.0846857, addr.NortheastBoundary.Longitude, Tol);
                        Verify.AreEqual(37.4211302, addr.SouthwestBoundary.Latitude, Tol);
                        Verify.AreEqual(-122.0862051, addr.SouthwestBoundary.Longitude, Tol);

                        Verify.IsNotNull(addr.PartialMatch);
                        Verify.IsTrue(addr.PartialMatch.Value);
                    }),

                    Case(suite, "LocationOnlyHasNullBoundaries",
                        "A result with a location but no bounds sets coordinates and leaves boundaries null", () =>
                    {
                        GoogleMapsResponse resp = Json.Deserialize<GoogleMapsResponse>(Fixtures.GeocodeOkLocationOnly);
                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);

                        Verify.AreEqual("Paris", addr.City);
                        Verify.AreEqual("France", addr.Country);
                        Verify.AreEqual("FR", addr.CountryAbbreviated);
                        Verify.IsNotNull(addr.Latitude);
                        Verify.AreEqual(48.856614, addr.Latitude.Value, Tol);
                        Verify.AreEqual(2.3522219, addr.Longitude.Value, Tol);
                        Verify.IsNull(addr.NortheastBoundary);
                        Verify.IsNull(addr.SouthwestBoundary);
                        Verify.IsNull(addr.PartialMatch);
                        Verify.IsNull(addr.StreetNumber);
                    }),

                    Case(suite, "PartialMatchFalseParsed",
                        "An explicit partial_match of false is preserved as false (not null)", () =>
                    {
                        Result result = new Result
                        {
                            FormattedAddress = "Somewhere Exact",
                            PartialMatch = false,
                            Geometry = new GeometryComponent { Location = Coord(10.0, 20.0) }
                        };

                        GoogleMapsResponse resp = new GoogleMapsResponse
                        {
                            Status = "OK",
                            Results = new List<Result> { result }
                        };

                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);
                        Verify.IsNotNull(addr.PartialMatch);
                        Verify.IsFalse(addr.PartialMatch.Value);
                    }),

                    Case(suite, "BoundsWithoutLocation",
                        "Bounds without a location populate boundaries and leave coordinates null", () =>
                    {
                        Result result = new Result
                        {
                            FormattedAddress = "Region",
                            Geometry = new GeometryComponent
                            {
                                Bounds = new Boundary
                                {
                                    Northeast = Coord(1.5, 2.5),
                                    Southwest = Coord(-1.5, -2.5)
                                },
                                Location = null
                            }
                        };

                        GoogleMapsResponse resp = new GoogleMapsResponse
                        {
                            Status = "OK",
                            Results = new List<Result> { result }
                        };

                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);
                        Verify.IsNotNull(addr.NortheastBoundary);
                        Verify.IsNotNull(addr.SouthwestBoundary);
                        Verify.AreEqual(1.5, addr.NortheastBoundary.Latitude, Tol);
                        Verify.AreEqual(-2.5, addr.SouthwestBoundary.Longitude, Tol);
                        Verify.IsNull(addr.Latitude);
                        Verify.IsNull(addr.Longitude);
                    }),

                    Case(suite, "OnlyFirstResultUsed",
                        "Only the first result in the response is flattened", () =>
                    {
                        Result first = new Result { FormattedAddress = "First" };
                        first.AddressComponents = new List<AddressComponent>
                        {
                            Comp("California", "CA", "administrative_area_level_1", "political")
                        };

                        Result second = new Result { FormattedAddress = "Second" };
                        second.AddressComponents = new List<AddressComponent>
                        {
                            Comp("Nevada", "NV", "administrative_area_level_1", "political")
                        };

                        GoogleMapsResponse resp = new GoogleMapsResponse
                        {
                            Status = "OK",
                            Results = new List<Result> { first, second }
                        };

                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);
                        Verify.AreEqual("First", addr.FormattedAddress);
                        Verify.AreEqual("California", addr.State);
                        Verify.AreEqual("CA", addr.StateAbbreviated);
                    }),

                    Case(suite, "EmptyAddressComponentsStillSetsCoordinates",
                        "A result with no address components still sets formatted address and coordinates", () =>
                    {
                        Result result = new Result
                        {
                            FormattedAddress = "Just Coordinates",
                            AddressComponents = new List<AddressComponent>(),
                            Geometry = new GeometryComponent { Location = Coord(51.5, -0.12) }
                        };

                        GoogleMapsResponse resp = new GoogleMapsResponse
                        {
                            Status = "OK",
                            Results = new List<Result> { result }
                        };

                        GoogleMapsAddress addr = new GoogleMapsAddress(resp);
                        Verify.AreEqual("Just Coordinates", addr.FormattedAddress);
                        Verify.IsNotNull(addr.Latitude);
                        Verify.AreEqual(51.5, addr.Latitude.Value, Tol);
                        Verify.IsNull(addr.City);
                        Verify.IsNull(addr.State);
                    }),
                });
        }
    }
}
