using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Mobile.Admin.Client.Core.Models;
using Edison.Mobile.Common.WiFi;

namespace Edison.Mobile.Admin.Client.Core.Ioc
{
    public interface IOnboardingRestService
    {
        Task<ResultCommandNetworkStatus> ConnectToNetwork(RequestNetworkInformationModel networkInformationModel);
        Task<IEnumerable<WifiNetwork>> GetAvailableWifiNetworks();
        Task<ResultCommandGetDeviceId> GetDeviceId();
        Task<ResultCommandGenerateCSR> GetGeneratedCSR();
        Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice provisionDeviceCommand);
        Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType setDeviceTypeCommand);
    }
}