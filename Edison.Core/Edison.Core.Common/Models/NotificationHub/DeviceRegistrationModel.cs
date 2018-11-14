using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Core.Common.Models
{
    public class DeviceRegistrationModel
    {
        public DeviceRegistrationModel(string identifier, string platform, string userEmail, IEnumerable<string> tags = null)
        {
            Identifier = identifier;
            Platform = platform;
            UserEmail = userEmail;
            Tags = tags;
        }
        public string Identifier { get; set; }
        public string Platform { get; set; }
        public string UserEmail { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
