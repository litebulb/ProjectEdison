using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Models;

namespace Edison.Devices.Onboarding.Client.Interfaces
{
    public interface ICommandsHelper
    {
        Task<ResultCommandGetDeviceId> GetDeviceId();
        Task<ResultCommandGenerateCSR> GenerateCSR();
        Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice requestProvisionDevice);
        Task<ResultCommandListFirmwares> ListFirmwares();
        Task<ResultCommandSoftAPSettings> GetAccessPointSettings();
        Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType requestSetDeviceType);
        Task<ResultCommandAvailableNetworks> RequestGetAvailableNetworks();
        Task<ResultCommandNetworkStatus> ConnectToClientNetwork(RequestCommandConnectToNetwork requestConnectToNetwork);
        Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys requestSetDeviceSecretKeys);
        Task<ResultCommandNetworkStatus> DisconnectFromClientNetwork(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork);
    }
}
