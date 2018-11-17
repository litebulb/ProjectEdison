using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Models;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface IDeviceApiClient
    {
        Task<ResultCommandGetDeviceId> GetDeviceId();
        Task<ResultCommandGenerateCSR> GetGeneratedCSR();
        Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice requestProvisionDevice);
        Task<ResultCommandListFirmwares> GetFirmwares();
        Task<ResultCommandSoftAPSettings> GetAccessPointSettings();
        Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType requestSetDeviceType);
        Task<ResultCommandAvailableNetworks> GetAvailableNetworks();
        Task<ResultCommandNetworkStatus> ConnectToClientNetwork(RequestCommandConnectToNetwork requestConnectToNetwork);
        Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys requestSetDeviceSecretKeys);
        Task<ResultCommandNetworkStatus> DisconnectFromClientNetwork(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork);
    }
}
