using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Common.Models.CommandModels;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface IDeviceConfigurationCommandsHelper
    {
        Task<IEnumerable<string>> ListFirmwares();
        Task<bool> SetDeviceType(string deviceType);
    }
}
