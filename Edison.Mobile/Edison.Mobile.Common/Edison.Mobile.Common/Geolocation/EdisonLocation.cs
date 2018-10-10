using System;
namespace Edison.Mobile.Common.Geolocation
{
    public class EdisonLocation
    {
        public Double Latitude { get; set; }
        public Double Longitude { get; set; }
        public Double Altitude { get; set; } // meters
        public Double Speed { get; set; } // meters per second
        public Double Bearing { get; set; } // degrees east of true north
        public DateTime Time { get; set; }
    }
}
