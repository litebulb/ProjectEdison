using System;

namespace Edison.Core.Common.Models
{
    public class ResponseStartModel
    {
        public Guid ResponseId { get; set; }
        public Geolocation Geolocation { get; set; }
    }
}
