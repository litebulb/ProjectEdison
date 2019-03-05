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

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public class ServiceMocks
    {
        public static void Setup(ContainerBuilder builder)
        {

            builder.RegisterInstance<IOnboardingRestService>(new OnboardingRestServiceMock());

        }        

        public class OnboardingRestServiceMock : IOnboardingRestService
        {
            public async Task<ResultCommandNetworkStatus> ConnectToNetwork(RequestNetworkInformationModel networkInformationModel)
            {
                return new ResultCommandNetworkStatus() { IsSuccess = true, Status = "Connected" };
            }

            public async Task<IEnumerable<WifiNetwork>> GetAvailableWifiNetworks()
            {
                return new List<WifiNetwork>()
                {
                    new WifiNetwork(){ SSID = "SSID 1"},
                    new WifiNetwork(){ SSID = "SSID 2"},
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