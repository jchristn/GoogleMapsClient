using RestWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoogleMapsClient
{
    /// <summary>
    /// Google Maps timestamp.
    /// </summary>
    public class GoogleMapsTimestamp
    {
        #region Public-Members

        /// <summary>
        /// Daylight savings time offset.
        /// </summary>
        [JsonPropertyName("dstOffset")]
        public double DaylightSavingsTimeOffset { get; set; } = 0;

        /// <summary>
        /// Raw offset.
        /// </summary>
        [JsonPropertyName("rawOffset")]
        public double RawOffset { get; set; } = 0;

        /// <summary>
        /// Status.
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = "OK";

        /// <summary>
        /// Timezone identifier.
        /// </summary>
        [JsonPropertyName("timeZoneId")]
        public string TimezoneId { get; set; } = "America/Los_Angeles";

        /// <summary>
        /// Timezone name.
        /// </summary>
        [JsonPropertyName("timeZoneName")]
        public string TimezoneName { get; set; } = "Pacific Daylight Time";

        /// <summary>
        /// Local time.
        /// </summary>
        public DateTime LocalTime
        {
            get
            {
                return DateTime.UtcNow.AddSeconds(DaylightSavingsTimeOffset + RawOffset);
            }
        }

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate.
        /// </summary>
        public GoogleMapsTimestamp()
        {

        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
