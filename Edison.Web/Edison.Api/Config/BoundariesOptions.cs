using Edison.Core.Common.Models;

namespace Edison.Api.Config
{
    public class BoundariesOptions
    {
        public Geolocation GeolocationPoint { get; set; }
        public double Radius { get; set; }
    }
}
