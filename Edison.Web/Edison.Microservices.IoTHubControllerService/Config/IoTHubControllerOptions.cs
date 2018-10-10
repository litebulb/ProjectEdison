using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edison.IoTHubControllerService.Config
{
    public class IoTHubControllerOptions
    {
        public string IoTHubConnectionString { get; set; }
        public int JobTimeout { get; set; }
        public int DirectMethodTimeout { get; set; }
    }
}
