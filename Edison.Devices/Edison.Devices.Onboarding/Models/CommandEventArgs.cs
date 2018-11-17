using Edison.Devices.Onboarding.Common.Models;
using Windows.Foundation;

namespace Edison.Devices.Onboarding.Models
{
    internal class CommandEventArgs
    {
        public Command InputCommand { get; set; }
        public Command OutputCommand { get; set; }
        //public Deferral Deferral { get; set; }
    }
}
