using Edison.Core.Common.Models;

namespace Edison.Api.Config
{
    public class BoundariesOptions
    {
        public Geolocation Epicenter { get; set; }
        public double Radius { get; set; }
    }
}
