using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Client.Models;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Common.Models.CommandModels;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface IDeviceProvisionCommandsHelper
    {
        Task<string> GetDeviceId();
        Task<string> GenerateCSR();
        Task<bool> ProvisionDevice(DeviceCertificateModel deviceCertificateModel);
    }
}
