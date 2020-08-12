using System;
using System.IO;
using System.Net; 
using Newtonsoft.Json;

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
        /// Method to use when sending log messages.
        /// </summary>
        public Action<string> Logger = null;

        #endregion

        #region Private-Members

        private string _ApiKey = null;
        private string _BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json?sensor=false&key=";
        private string _Header = "[GoogleMaps] ";

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
        public Address QueryCoordinates(double latitude, double longitude)
        {
            string url = _BaseUrl + _ApiKey + "&latlng=" + latitude + "," + longitude;
            Logger?.Invoke(_Header + "QueryCoordinates " + latitude + "," + longitude);
            GoogleMapsResponse resp = GetGoogleMapsResponse(url);
            return new Address(resp);
        }

        /// <summary>
        /// Retrieve address details for a specified address.
        /// </summary>
        /// <param name="address">Address.</param>
        /// <returns>Address details.</returns>
        public Address QueryAddress(string address)
        {
            if (String.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            string url = _BaseUrl + _ApiKey + "&address=" + WebUtility.UrlEncode(address);
            Logger?.Invoke(_Header + "QueryAddress " + address);
            GoogleMapsResponse resp = GetGoogleMapsResponse(url);
            return new Address(resp);
        }

        #endregion

        #region Private-Methods

        private GoogleMapsResponse GetGoogleMapsResponse(string url)
        {
            Logger?.Invoke(_Header + "URL: " + url);

            WebRequest request = WebRequest.Create(url);
            using (WebResponse response = request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    byte[] respData = Common.StreamToBytes(stream);
                    return Common.DeserializeJson<GoogleMapsResponse>(respData);
                }
            }  
        }

        #endregion
    }
}
