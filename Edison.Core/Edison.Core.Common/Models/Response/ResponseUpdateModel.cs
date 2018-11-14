using Edison.Core.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class ResponseUpdateModel
    {
        public Guid ResponseId { get; set; }
        public Geolocation Geolocation { get; set; }
    }
}
