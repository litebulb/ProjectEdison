using Edison.Mobile.Admin.Client.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Edison.Mobile.Common.WiFi;
using System.Threading.Tasks;
using System.Linq;
using Edison.Mobile.Admin.Client.Core.Ioc;
using Autofac;
using Edison.Mobile.Admin.Client.Core.Models;
using RestSharp;
using Edison.Core.Common.Models;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public class ServiceMocks
    {
        public static void Setup(ContainerBuilder builder)
        {

            builder.RegisterInstance<IOnboardingRestService>(new OnboardingRestServiceMock());
            builder.RegisterInstance<IDeviceRestService>(new DeviceRestServiceMock());
            builder.RegisterInstance<IDeviceProvisioningRestService>(new DeviceProvisioningRestServiceMock());
        }

        public class DeviceProvisioningRestServiceMock : IDeviceProvisioningRestService
        {
            public Task<DeviceCertificateModel> GenerateDeviceCertificate(DeviceCertificateRequestModel deviceCertificateRequestModel)
            {
                return Task.FromResult(new DeviceCertificateModel()
                {
                    Certificate = "asdf",
                    DeviceType = "SmartBulb",
                    DpsIdScope = "",
                    DpsInstance = "asdf"                    
                });
            }

            public Task<DeviceSecretKeysModel> GenerateDeviceKeys(Guid deviceId, string ssidName)
            {
                return Task.FromResult(new DeviceSecretKeysModel()
                {
                    SSIDPassword = "Edison1234",
                    EncryptionKey = "Asdfasd",
                    PortalPassword = "Edison1234"
                });
            }

            public Task<DeviceSecretKeysModel> GetDeviceKeys(Guid deviceId)
            {
                return Task.FromResult(new DeviceSecretKeysModel()
                {
                    SSIDPassword = "Edison1234",
                    EncryptionKey = "Asdfasd",
                    PortalPassword = "Edison1234"
                });
            }
        }

        public class DeviceRestServiceMock : IDeviceRestService
        {
            public Task<DeviceModel> GetDevice(Guid deviceId)
            {
                return Task.FromResult(new DeviceModel()
                {
                    DeviceId = deviceId,                    
                });
            }

            public Task<IEnumerable<DeviceModel>> GetDevices(Geolocation geolocation = null)
            {
                return Task.FromResult<IEnumerable<DeviceModel>>(new List<DeviceModel>()
                {
                    new DeviceModel(){ DeviceId = Guid.NewGuid(), Name = "Fake Device" },
                    new DeviceModel(){ DeviceId = Guid.NewGuid(), Name = "Another Fake Device" },
                });
            }

            public Task<bool> UpdateDevice(DevicesUpdateTagsModel updateTagsModel)
            {
                return Task.FromResult(true);
            }
        }


        public class OnboardingRestServiceMock : IOnboardingRestService
        {
            public void AddAuthHeader(RestRequest request)
            {
                
            }

            public Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys command)
            {
                return Task.FromResult(new ResultCommand()
                {
                    IsSuccess = true
                });
            }

            public async Task<ResultCommandNetworkStatus> ConnectToNetwork(RequestNetworkInformationModel networkInformationModel)
            {
                return new ResultCommandNetworkStatus() { IsSuccess = true, Status = "Connected" };
            }

            public async Task<IEnumerable<AvailableNetwork>> GetAvailableWifiNetworks()
            {
                return new List<AvailableNetwork>()
                {
                    new AvailableNetwork(){ SSID = "SSID 1", AlreadyConnected = true},
                    new AvailableNetwork(){ SSID = "SSID 2"},
                };
            }

            public Task<ResultCommandGetDeviceId> GetDeviceId()
            {
                return Task.FromResult(new ResultCommandGetDeviceId()
                {
                    DeviceId = Guid.NewGuid(),
                    IsSuccess = true
                });
            }

            public Task<ResultCommandGenerateCSR> GetGeneratedCSR()
            {
                return Task.FromResult(new ResultCommandGenerateCSR()
                {
                    Csr = "random string",
                    IsSuccess = true
                });
            }

            public Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice provisionDeviceCommand)
            {
                return Task.FromResult(new ResultCommand()
                {
                    IsSuccess = true
                });
            }

            public void SetBasicAuthentication(string password)
            {
                
            }

            public Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType setDeviceTypeCommand)
            {
                return Task.FromResult(new ResultCommand()
                {
                    IsSuccess = true
                });
            }
        }
    }
}