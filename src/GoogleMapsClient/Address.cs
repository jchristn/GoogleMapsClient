using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleMapsClient
{
    /// <summary>
    /// Address parsed from Google Maps API response.
    /// </summary>
    public class Address
    {
        #region Public-Members

        /// <summary>
        /// An address to find.
        /// </summary>
        public string Query { get; set; } = null;

        /// <summary>
        /// The formatted string representation of the supplied query.
        /// </summary>
        public string FormattedAddress { get; set; } = null;

        /// <summary>
        /// The latitude of the address.
        /// </summary>
        public double? Latitude { get; set; } = null;

        /// <summary>
        /// The longitude of the address.
        /// </summary>
        public double? Longitude { get; set; } = null;

        /// <summary>
        /// The street number of the address.
        /// </summary>
        public string StreetNumber { get; set; } = null;

        /// <summary>
        /// The street name of the address.
        /// </summary>
        public string StreetName { get; set; } = null;

        /// <summary>
        /// The neighborhood associated with the address.
        /// </summary>
        public string Neighborhood { get; set; } = null;

        /// <summary>
        /// The city associated with the address.
        /// </summary>
        public string City { get; set; } = null;

        /// <summary>
        /// The county associated with the address.
        /// </summary>
        public string County { get; set; } = null;

        /// <summary>
        /// The state associated with the address.
        /// </summary>
        public string State { get; set; } = null;

        /// <summary>
        /// The abbreviated state associated with the address.
        /// </summary>
        public string StateAbbreviated { get; set; } = null;

        /// <summary>
        /// The country associated with the address.
        /// </summary>
        public string Country { get; set; } = null;

        /// <summary>
        /// The abbreviated country associated with the address.
        /// </summary>
        public string CountryAbbreviated { get; set; } = null;

        /// <summary>
        /// The postal code suffix associated with the address.
        /// </summary>
        public string PostalSuffix { get; set; } = null;

        /// <summary>
        /// The postal code associated with the address.
        /// </summary>
        public string Postal { get; set; } = null;

        /// <summary>
        /// Coordinates for the Northeast boundary.
        /// </summary>
        public Coordinates NortheastBoundary { get; set; } = null;

        /// <summary>
        /// Coordinates for the Southwest boundary.
        /// </summary>
        public Coordinates SouthwestBoundary { get; set; } = null;

        /// <summary>
        /// Response object provided by Google Maps endpoint.
        /// </summary>
        public GoogleMapsResponse GoogleResponse { get; set; } = null;

        #endregion

        #region Private-Members

        private GoogleMapsResponse _GoogleMapsResponse = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public Address()
        {

        }

        /// <summary>
        /// Instantiate the response.
        /// </summary>
        /// <param name="resp">Google Maps response.</param>
        public Address(GoogleMapsResponse resp)
        {
            if (resp == null) throw new ArgumentNullException(nameof(resp));

            _GoogleMapsResponse = resp;

            if (resp.Status.Equals("OK") 
                && resp.Results != null 
                && resp.Results.Count > 0)
            {
                GoogleMapsResponse.Result result = resp.Results[0];

                FormattedAddress = result.FormattedAddress;

                if (result.AddressComponents != null
                    && result.AddressComponents.Count > 0)
                {
                    foreach (GoogleMapsResponse.Result.AddressComponent comp in result.AddressComponents)
                    {
                        if (comp.Types != null && comp.Types.Count > 0)
                        {
                            foreach (string currType in comp.Types)
                            {
                                if (currType.Equals("street_number")) StreetNumber = comp.LongName;

                                if (currType.Equals("route")) StreetName = comp.LongName;

                                if (currType.Equals("neighborhood")) Neighborhood = comp.LongName;

                                if (currType.Equals("locality")) City = comp.LongName;

                                if (currType.Equals("administrative_area_level_2")) County = comp.LongName;

                                if (currType.Equals("administrative_area_level_1"))
                                {
                                    State = comp.LongName;
                                    StateAbbreviated = comp.ShortName;
                                }

                                if (currType.Equals("country"))
                                {
                                    Country = comp.LongName;
                                    CountryAbbreviated = comp.ShortName;
                                }

                                if (currType.Equals("postal_code")) Postal = comp.LongName;

                                if (currType.Equals("postal_code_suffix")) PostalSuffix = comp.LongName; 
                            }
                        }
                    }
                }

                if (result.Geometry != null)
                {
                    if (result.Geometry.Bounds != null)
                    {
                        if (result.Geometry.Bounds.Northeast != null)
                        {
                            NortheastBoundary = new Coordinates();
                            NortheastBoundary.Latitude = result.Geometry.Bounds.Northeast.Latitude;
                            NortheastBoundary.Longitude = result.Geometry.Bounds.Northeast.Longitude; 
                        }

                        if (result.Geometry.Bounds.Southwest != null)
                        {
                            SouthwestBoundary = new Coordinates();
                            SouthwestBoundary.Latitude = result.Geometry.Bounds.Southwest.Latitude;
                            SouthwestBoundary.Longitude = result.Geometry.Bounds.Southwest.Longitude;
                        }
                    }

                    if (result.Geometry.Location != null)
                    {
                        Latitude = result.Geometry.Location.Latitude;
                        Longitude = result.Geometry.Location.Longitude;
                    } 
                }
            }
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Members

        #endregion
    }
}
