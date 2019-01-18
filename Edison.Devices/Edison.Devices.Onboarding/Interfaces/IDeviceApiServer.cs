using Edison.Devices.Onboarding.Models;
using System;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Interfaces
{
    internal interface IDeviceApiServer
    {
        event Func<CommandEventArgs, Task> CommandReceived;
        void Start();
    }
}
