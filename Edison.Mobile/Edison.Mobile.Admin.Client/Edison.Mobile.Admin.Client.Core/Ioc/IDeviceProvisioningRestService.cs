using System;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public interface IDeviceProvisioningRestService
    {
        Task<DeviceCertificateModel> GenerateDeviceCertificate(DeviceCertificateRequestModel deviceCertificateRequestModel);
        Task<DeviceSecretKeysModel> GenerateDeviceKeys(Guid deviceId);
        Task<DeviceSecretKeysModel> GetDeviceKeys(Guid deviceId);
    }
}