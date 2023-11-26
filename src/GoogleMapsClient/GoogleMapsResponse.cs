using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoogleMapsClient
{
    /// <summary>
    /// Response from Google Maps API endpoint.
    /// </summary>
    public class GoogleMapsResponse
    {
        /// <summary>
        /// List of results.
        /// </summary>
        [JsonPropertyName("results")]
        public List<Result> Results { get; set; } = new List<Result>();

        /// <summary>
        /// Status of the operation.  
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = null;

        /// <summary>
        /// Result.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// Address components.
            /// </summary>
            [JsonPropertyName("address_components")]
            public List<AddressComponent> AddressComponents { get; set; } = new List<AddressComponent>();

            /// <summary>
            /// Formatted address.
            /// </summary>
            [JsonPropertyName("formatted_address")]
            public string FormattedAddress { get; set; } = null;

            /// <summary>
            /// Viewport, boundaries, and location coordinates.
            /// </summary>
            [JsonPropertyName("geometry")]
            public GeometryComponent Geometry { get; set; } = null;

            /// <summary>
            /// Partial Match.
            /// </summary>
            [JsonPropertyName("partial_match")]
            public bool? PartialMatch { get; set; } = null;

            /// <summary>
            /// Google place ID.
            /// </summary>
            [JsonPropertyName("place_id")]
            public string PlaceId { get; set; } = null;

            /// <summary>
            /// The type of location represented by these results.
            /// </summary>
            [JsonPropertyName("types")]
            public List<string> Types { get; set; } = new List<string>();

            /// <summary>
            /// Address component.
            /// </summary>
            public class AddressComponent
            {
                /// <summary>
                /// The long name of the address component.
                /// </summary>
                [JsonPropertyName("long_name")]
                public string LongName { get; set; } = null;
                
                /// <summary>
                /// The short name of the address component.
                /// </summary>
                [JsonPropertyName("short_name")]
                public string ShortName { get; set; } = null;

                /// <summary>
                /// The types represented by the address component.
                /// </summary>
                [JsonPropertyName("types")]
                public List<string> Types { get; set; } = new List<string>();
            }

            /// <summary>
            /// Viewport, boundaries, and location coordinates.
            /// </summary>
            public class GeometryComponent
            {
                /// <summary>
                /// The Northeast and Southwest boundary coordinates.
                /// </summary>
                [JsonPropertyName("bounds")]
                public Boundary Bounds { get; set; } = null;

                /// <summary>
                /// The location coordinates.
                /// </summary>
                [JsonPropertyName("location")]
                public GoogleCoordinates Location { get; set; } = null;

                /// <summary>
                /// The type of location.
                /// </summary>
                [JsonPropertyName("location_type")]
                public string LocationType { get; set; } = null;

                /// <summary>
                /// The Northeast and Southwest viewport boundary coordinates.
                /// </summary>
                [JsonPropertyName("viewport")]
                public Boundary Viewport { get; set; } = null;

                /// <summary>
                /// Boundary coordinates.
                /// </summary>
                public class Boundary
                {
                    /// <summary>
                    /// Northeast coordinates.
                    /// </summary>
                    [JsonPropertyName("northeast")]
                    public GoogleCoordinates Northeast { get; set; } = null;
                      
                    /// <summary>
                    /// Southwest coordinates.
                    /// </summary>
                    [JsonPropertyName("southwest")]
                    public GoogleCoordinates Southwest { get; set; } = null;
                }

                /// <summary>
                /// Coordinates.
                /// </summary>
                public class GoogleCoordinates
                {
                    /// <summary>
                    /// Latitude.
                    /// </summary>
                    [JsonPropertyName("lat")]
                    public double Latitude { get; set; } = 0;

                    /// <summary>
                    /// Longitude.
                    /// </summary>
                    [JsonPropertyName("lng")]
                    public double Longitude { get; set; } = 0;
                }
            }
        }
    }
}
