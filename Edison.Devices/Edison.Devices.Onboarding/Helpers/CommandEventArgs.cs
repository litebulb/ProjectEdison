using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Edison.Devices.Onboarding.Helpers
{
    internal class CommandEventArgs
    {
        public Command InputCommand { get; set; }
        public Command OutputCommand { get; set; }
        public Deferral Deferral { get; set; }
    }
}
