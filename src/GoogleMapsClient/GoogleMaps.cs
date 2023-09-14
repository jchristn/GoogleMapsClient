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
        /// Method to use when sending log messages.
        /// </summary>
        public Action<string> Logger { get; set; } = null;

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
            GoogleMapsResponse resp = GetGoogleMapsResponse(HttpMethod.Get, url);
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
            GoogleMapsResponse resp = GetGoogleMapsResponse(HttpMethod.Get, url);
            return new Address(resp);
        }

        /// <summary>
        /// Generate a timestamp for a given set of coordinates.
        /// </summary>
        /// <param name="latitude">Latitude.</param>
        /// <param name="longitude">Longitude.</param>
        /// <param name="timestamp">Timestamp for which the local timestamp should be retrieved.</param>
        /// <param name="timezone">Timezone string.</param>
        /// <returns>Local timestamp.</returns>
        public DateTime LocalTimestamp(double latitude, double longitude, DateTime timestamp, out string timezone)
        {
            timezone = "Universal Time Coordinated (UTC)";

            timestamp = timestamp.ToUniversalTime();
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = timestamp.ToUniversalTime() - origin;
            double ts = Math.Floor(diff.TotalSeconds);

            string url = "https://maps.googleapis.com/maps/api/timezone/json?location=" + latitude + "," + longitude + "&key=" + _ApiKey + "&timestamp=" + ts;
            string result = GetRestResponse(HttpMethod.Get, url);

            GoogleMapsTimestamp data = SerializationHelper.DeserializeJson<GoogleMapsTimestamp>(result);
            timezone = data.TimezoneName;
            return timestamp.AddSeconds(data.DaylightSavingsTimeOffset + data.RawOffset);
        }

        /// <summary>
        /// Retrieve the local timestamp for a given address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="timestamp">Timestamp for which the local timestamp should be retrieved.</param>
        /// <param name="timezone">Timezone string.</param>
        /// <returns>Local timestamp.</returns>
        public DateTime LocalTimestamp(string address, DateTime timestamp, out string timezone)
        {
            if (String.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            Address addr = QueryAddress(address);
            return LocalTimestamp(
                Convert.ToDouble(addr.Latitude), 
                Convert.ToDouble(addr.Longitude), 
                timestamp, 
                out timezone);
        }

        #endregion

        #region Private-Methods

        private GoogleMapsResponse GetGoogleMapsResponse(HttpMethod method, string url, string body = null)
        {
            return SerializationHelper.DeserializeJson<GoogleMapsResponse>(GetRestResponse(method, url, body));
        }

        private async Task<GoogleMapsResponse> GetGoogleMapsResponseAsync(HttpMethod method, string url, string body = null)
        {
            return SerializationHelper.DeserializeJson<GoogleMapsResponse>(await GetRestResponseAsync(method, url, body));
        }

        private string GetRestResponse(HttpMethod method, string url, string body = null)
        {
            string loggedUrl = url;
            loggedUrl = loggedUrl.Replace(_ApiKey, "[redacted]");
            Logger?.Invoke(_Header + method.ToString() + " " + url);

            RestRequest req = new RestRequest(url, method);
            RestResponse resp = null;

            if (!String.IsNullOrEmpty(body)) resp = req.Send(body);
            else resp = req.Send();

            string data = resp.DataAsString;
            Logger?.Invoke(_Header + "response:" + Environment.NewLine + resp.DataAsString);

            return resp.DataAsString;
        }

        private async Task<string> GetRestResponseAsync(HttpMethod method, string url, string body = null)
        {
            RestRequest req = new RestRequest(url, method);
            RestResponse resp = null;

            if (!String.IsNullOrEmpty(body)) resp = await req.SendAsync(body);
            else resp = await req.SendAsync();

            string data = resp.DataAsString;
            Logger?.Invoke(_Header + "response:" + Environment.NewLine + resp.DataAsString);

            return resp.DataAsString;
        }

        #endregion
    }
}
