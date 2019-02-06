using System;
namespace Edison.Mobile.Common.Geo
{
    public class EdisonLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; } // meters
        public double Speed { get; set; } // meters per second
        public double Bearing { get; set; } // degrees east of true north
        public DateTime Time { get; set; }


        public EdisonLocation() { }

        public EdisonLocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
