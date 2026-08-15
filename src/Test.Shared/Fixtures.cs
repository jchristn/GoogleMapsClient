namespace Test.Shared
{
    /// <summary>
    /// Canned JSON payloads that mirror real Google Maps Geocoding and Time Zone API responses.
    /// These allow the full deserialization and parsing pipeline to be exercised deterministically,
    /// with no network access or API key required.
    /// </summary>
    internal static class Fixtures
    {
        /// <summary>
        /// A complete, successful geocoding response exercising every address component type the
        /// library maps, plus geometry location and bounds and a partial-match flag.
        /// </summary>
        internal const string GeocodeOk = @"{
  ""results"": [
    {
      ""address_components"": [
        { ""long_name"": ""1600"", ""short_name"": ""1600"", ""types"": [""street_number""] },
        { ""long_name"": ""Amphitheatre Parkway"", ""short_name"": ""Amphitheatre Pkwy"", ""types"": [""route""] },
        { ""long_name"": ""Shoreline West"", ""short_name"": ""Shoreline West"", ""types"": [""neighborhood"", ""political""] },
        { ""long_name"": ""Mountain View"", ""short_name"": ""Mountain View"", ""types"": [""locality"", ""political""] },
        { ""long_name"": ""Santa Clara County"", ""short_name"": ""Santa Clara County"", ""types"": [""administrative_area_level_2"", ""political""] },
        { ""long_name"": ""California"", ""short_name"": ""CA"", ""types"": [""administrative_area_level_1"", ""political""] },
        { ""long_name"": ""United States"", ""short_name"": ""US"", ""types"": [""country"", ""political""] },
        { ""long_name"": ""94043"", ""short_name"": ""94043"", ""types"": [""postal_code""] },
        { ""long_name"": ""1351"", ""short_name"": ""1351"", ""types"": [""postal_code_suffix""] }
      ],
      ""formatted_address"": ""1600 Amphitheatre Pkwy, Mountain View, CA 94043, USA"",
      ""geometry"": {
        ""bounds"": {
          ""northeast"": { ""lat"": 37.4229909, ""lng"": -122.0846857 },
          ""southwest"": { ""lat"": 37.4211302, ""lng"": -122.0862051 }
        },
        ""location"": { ""lat"": 37.4220656, ""lng"": -122.0840897 },
        ""location_type"": ""ROOFTOP"",
        ""viewport"": {
          ""northeast"": { ""lat"": 37.4234095, ""lng"": -122.0827964 },
          ""southwest"": { ""lat"": 37.4207116, ""lng"": -122.0880943 }
        }
      },
      ""partial_match"": true,
      ""place_id"": ""ChIJ2eUgeAK6j4ARbn5u_wAGqWA"",
      ""types"": [""street_address""]
    }
  ],
  ""status"": ""OK""
}";

        /// <summary>
        /// A minimal successful response that has a geometry location but no bounds, used to verify
        /// that latitude/longitude are populated while the boundary properties remain null.
        /// </summary>
        internal const string GeocodeOkLocationOnly = @"{
  ""results"": [
    {
      ""address_components"": [
        { ""long_name"": ""Paris"", ""short_name"": ""Paris"", ""types"": [""locality"", ""political""] },
        { ""long_name"": ""France"", ""short_name"": ""FR"", ""types"": [""country"", ""political""] }
      ],
      ""formatted_address"": ""Paris, France"",
      ""geometry"": {
        ""location"": { ""lat"": 48.856614, ""lng"": 2.3522219 },
        ""location_type"": ""APPROXIMATE""
      },
      ""place_id"": ""ChIJD7fiBh9u5kcRYJSMaMOCCwQ"",
      ""types"": [""locality"", ""political""]
    }
  ],
  ""status"": ""OK""
}";

        /// <summary>A well-formed response indicating the query matched nothing.</summary>
        internal const string GeocodeZeroResults = @"{ ""results"": [], ""status"": ""ZERO_RESULTS"" }";

        /// <summary>A response indicating an invalid or missing API key.</summary>
        internal const string GeocodeRequestDenied = @"{ ""results"": [], ""status"": ""REQUEST_DENIED"", ""error_message"": ""The provided API key is invalid."" }";

        /// <summary>A successful Time Zone API response for Pacific Daylight Time.</summary>
        internal const string TimezoneOk = @"{
  ""dstOffset"": 3600,
  ""rawOffset"": -28800,
  ""status"": ""OK"",
  ""timeZoneId"": ""America/Los_Angeles"",
  ""timeZoneName"": ""Pacific Daylight Time""
}";
    }
}
