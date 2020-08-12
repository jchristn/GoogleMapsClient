using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

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
        [JsonProperty("results")]        
        public List<Result> Results = new List<Result>();

        /// <summary>
        /// Status of the operation.  
        /// </summary>
        [JsonProperty("status")]
        public string Status = null;

        /// <summary>
        /// Result.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// Address components.
            /// </summary>
            [JsonProperty("address_components")]
            public List<AddressComponent> AddressComponents = new List<AddressComponent>();

            /// <summary>
            /// Formatted address.
            /// </summary>
            [JsonProperty("formatted_address")]
            public string FormattedAddress = null;

            /// <summary>
            /// Viewport, boundaries, and location coordinates.
            /// </summary>
            [JsonProperty("geometry")]
            public GeometryComponent Geometry = null;

            /// <summary>
            /// Google place ID.
            /// </summary>
            [JsonProperty("place_id")]
            public string PlaceId = null;

            /// <summary>
            /// The type of location represented by these results.
            /// </summary>
            [JsonProperty("types")]
            public List<string> Types = new List<string>();

            /// <summary>
            /// Address component.
            /// </summary>
            public class AddressComponent
            {
                /// <summary>
                /// The long name of the address component.
                /// </summary>
                [JsonProperty("long_name")]
                public string LongName = null;
                
                /// <summary>
                /// The short name of the address component.
                /// </summary>
                [JsonProperty("short_name")]
                public string ShortName = null;

                /// <summary>
                /// The types represented by the address component.
                /// </summary>
                [JsonProperty("types")]
                public List<string> Types = new List<string>();
            }

            /// <summary>
            /// Viewport, boundaries, and location coordinates.
            /// </summary>
            public class GeometryComponent
            {
                /// <summary>
                /// The Northeast and Southwest boundary coordinates.
                /// </summary>
                [JsonProperty("bounds")]
                public Boundary Bounds = null;

                /// <summary>
                /// The location coordinates.
                /// </summary>
                [JsonProperty("location")]
                public GoogleCoordinates Location = null;

                /// <summary>
                /// The type of location.
                /// </summary>
                [JsonProperty("location_type")]
                public string LocationType = null;

                /// <summary>
                /// The Northeast and Southwest viewport boundary coordinates.
                /// </summary>
                [JsonProperty("viewport")]
                public Boundary Viewport = null;

                /// <summary>
                /// Boundary coordinates.
                /// </summary>
                public class Boundary
                {
                    /// <summary>
                    /// Northeast coordinates.
                    /// </summary>
                    [JsonProperty("northeast")]
                    public GoogleCoordinates Northeast = null;
                      
                    /// <summary>
                    /// Southwest coordinates.
                    /// </summary>
                    [JsonProperty("southwest")]
                    public GoogleCoordinates Southwest = null;
                }

                /// <summary>
                /// Coordinates.
                /// </summary>
                public class GoogleCoordinates
                {
                    /// <summary>
                    /// Latitude.
                    /// </summary>
                    [JsonProperty("lat")]
                    public double Latitude = 0;

                    /// <summary>
                    /// Longitude.
                    /// </summary>
                    [JsonProperty("lng")]
                    public double Longitude = 0;
                }
            }
        }
    }
}
