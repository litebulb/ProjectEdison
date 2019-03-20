using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Common.WiFi;
using RestSharp;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public interface IOnboardingRestService
    {
        Task<ResultCommandNetworkStatus> ConnectToNetwork(RequestNetworkInformationModel networkInformationModel);
        Task<IEnumerable<AvailableNetwork>> GetAvailableWifiNetworks();
        Task<ResultCommandGetDeviceId> GetDeviceId();
        Task<ResultCommandGenerateCSR> GetGeneratedCSR();
        Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice provisionDeviceCommand);
        Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType setDeviceTypeCommand);
        Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys command);
        void SetBasicAuthentication(string password);
    }
}