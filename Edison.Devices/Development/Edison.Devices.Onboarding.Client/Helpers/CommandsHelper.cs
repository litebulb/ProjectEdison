using Edison.Devices.Onboarding.Client.Interfaces;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Models;

namespace Edison.Devices.Onboarding.Client.Helpers
{
    public class CommandsHelper : ICommandsHelper
    {
        private readonly IStreamSockerClient _socketClient;

        public CommandsHelper(IStreamSockerClient socketClient)
        {
            _socketClient = socketClient;
        }

        /// <summary>
        /// Retrieve DeviceId of the Device
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandGetDeviceId> GetDeviceId()
        {
            var resultGetDevice = await _socketClient.SendCommand<ResultCommandGetDeviceId>
                (CommandsEnum.GetDeviceId, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultGetDevice;
        }

        /// <summary>
        /// Command Provision Device
        /// The Certificate Model must be retrieve from /Certificates/DeviceProvisioning/ endpoint
        /// </summary>
        /// <param name="deviceCertificateModel">DeviceCertificateModel certificate</param>
        /// <returns>True if autoprovisioning succeeded</returns>
        public async Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice requestProvisionDevice)
        {
            var resultProvisionDevice = await _socketClient.SendCommand<RequestCommandProvisionDevice, ResultCommand>
                (CommandsEnum.ProvisionDevice, requestProvisionDevice, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultProvisionDevice;
        }

        /// <summary>
        /// Command Generate CSR
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandGenerateCSR> GenerateCSR()
        {
            var resultGenerateCSR = await _socketClient.SendCommand<ResultCommandGenerateCSR>
                (CommandsEnum.GenerateCSR, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultGenerateCSR;
        }

        /// <summary>
        /// Return a list of available firmwares
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandListFirmwares> ListFirmwares()
        {
            var resultListFirmwares = await _socketClient.SendCommand<ResultCommandListFirmwares>
                (CommandsEnum.ListFirmwares, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultListFirmwares;
        }

        /// <summary>
        /// Return AP settings
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandSoftAPSettings> GetAccessPointSettings()
        {
            var resultGetAccessPointSettings = await _socketClient.SendCommand<ResultCommandSoftAPSettings>
                (CommandsEnum.GetAccessPointSettings, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultGetAccessPointSettings;
        }

        /// <summary>
        /// Set Device Type and start the proper firmware
        /// </summary>
        /// <param name="requestSetDeviceType"></param>
        /// <returns></returns>
        public async Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType requestSetDeviceType)
        {
            var resultSetDeviceType = await _socketClient.SendCommand<RequestCommandSetDeviceType, ResultCommand>
                (CommandsEnum.SetDeviceType, requestSetDeviceType, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultSetDeviceType;
        }

        /// <summary>
        /// Set Device Secret Keys. Access Point and Encryption key need to be refresh after that
        /// </summary>
        /// <param name="requestSetDeviceType"></param>
        /// <returns></returns>
        public async Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys requestSetDeviceSecretKeys)
        {
            var resultSetDeviceSecretKeys = await _socketClient.SendCommand<RequestCommandSetDeviceSecretKeys, ResultCommand>
                (CommandsEnum.SetDeviceSecretKeys, requestSetDeviceSecretKeys, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultSetDeviceSecretKeys;
        }

        /// <summary>
        /// Get available wifi networks
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandAvailableNetworks> RequestGetAvailableNetworks()
        {
            var resultGetAvailableNetworks = await _socketClient.SendCommand<ResultCommandAvailableNetworks>
                (CommandsEnum.GetAvailableNetworks, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultGetAvailableNetworks;
        }

        /// <summary>
        /// Order the device to connect to specified network
        /// </summary>
        /// <param name="requestConnectToNetwork"></param>
        /// <returns></returns>
        public async Task<ResultCommandNetworkStatus> ConnectToClientNetwork(RequestCommandConnectToNetwork requestConnectToNetwork)
        {
            var resultConnectToNetwork = await _socketClient.SendCommand<RequestCommandConnectToNetwork, ResultCommandNetworkStatus>
                (CommandsEnum.ConnectToNetwork, requestConnectToNetwork, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultConnectToNetwork;
        }

        /// <summary>
        /// Order the device to disconnected from specified network
        /// </summary>
        /// <param name="requestDisconnectFromNetwork"></param>
        /// <returns></returns>
        public async Task<ResultCommandNetworkStatus> DisconnectFromClientNetwork(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork)
        {
            var resultDisconnectFromNetwork = await _socketClient.SendCommand<RequestCommandDisconnectFromNetwork, ResultCommandNetworkStatus>
                (CommandsEnum.DisconnectFromNetwork, requestDisconnectFromNetwork, Common.Helpers.SharedConstants.DEFAULT_SOCKET_PASSPHRASE);
            return resultDisconnectFromNetwork;
        }
    }
}
