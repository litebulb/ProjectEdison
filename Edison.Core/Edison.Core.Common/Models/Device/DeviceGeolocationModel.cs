namespace Edison.Core.Common.Models
{
    public class DeviceGeolocationModel
    {
        public Geolocation ResponseGeolocationPointLocation { get; set; }
        public double Radius { get; set; }
        public string DeviceType { get; set; }
    }
}
