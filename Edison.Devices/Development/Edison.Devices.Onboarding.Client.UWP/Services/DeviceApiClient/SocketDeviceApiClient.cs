using Edison.Devices.Onboarding.Client.Interfaces;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Common.Helpers;

namespace Edison.Devices.Onboarding.Client.UWP
{
    public class SocketDeviceApiClient : IDeviceApiClient
    {
        private readonly IStreamClient _streamClient;

        public SocketDeviceApiClient()
        {
            _streamClient = new StreamSocketClient(SharedConstants.DEVICE_DEBUG_API_IP, SharedConstants.DEVICE_API_PORT);
        }

        /// <summary>
        /// Retrieve DeviceId of the Device
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandGetDeviceId> GetDeviceId()
        {
            var resultGetDevice = await _streamClient.SendCommand<ResultCommandGetDeviceId>
                (CommandsEnum.GetDeviceId, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
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
            var resultProvisionDevice = await _streamClient.SendCommand<RequestCommandProvisionDevice, ResultCommand>
                (CommandsEnum.ProvisionDevice, requestProvisionDevice, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultProvisionDevice;
        }

        /// <summary>
        /// Command Generate CSR
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandGenerateCSR> GetGeneratedCSR()
        {
            var resultGenerateCSR = await _streamClient.SendCommand<ResultCommandGenerateCSR>
                (CommandsEnum.GetGeneratedCSR, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultGenerateCSR;
        }

        /// <summary>
        /// Return a list of available firmwares
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandListFirmwares> GetFirmwares()
        {
            var resultListFirmwares = await _streamClient.SendCommand<ResultCommandListFirmwares>
                (CommandsEnum.GetFirmwares, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultListFirmwares;
        }

        /// <summary>
        /// Return AP settings
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandSoftAPSettings> GetAccessPointSettings()
        {
            var resultGetAccessPointSettings = await _streamClient.SendCommand<ResultCommandSoftAPSettings>
                (CommandsEnum.GetAccessPointSettings, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultGetAccessPointSettings;
        }

        /// <summary>
        /// Set Device Type and start the proper firmware
        /// </summary>
        /// <param name="requestSetDeviceType"></param>
        /// <returns></returns>
        public async Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType requestSetDeviceType)
        {
            var resultSetDeviceType = await _streamClient.SendCommand<RequestCommandSetDeviceType, ResultCommand>
                (CommandsEnum.SetDeviceType, requestSetDeviceType, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultSetDeviceType;
        }

        /// <summary>
        /// Set Device Secret Keys. Access Point and Encryption key need to be refresh after that
        /// </summary>
        /// <param name="requestSetDeviceType"></param>
        /// <returns></returns>
        public async Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys requestSetDeviceSecretKeys)
        {
            var resultSetDeviceSecretKeys = await _streamClient.SendCommand<RequestCommandSetDeviceSecretKeys, ResultCommand>
                (CommandsEnum.SetDeviceSecretKeys, requestSetDeviceSecretKeys, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultSetDeviceSecretKeys;
        }

        /// <summary>
        /// Get available wifi networks
        /// </summary>
        /// <returns></returns>
        public async Task<ResultCommandAvailableNetworks> GetAvailableNetworks()
        {
            var resultGetAvailableNetworks = await _streamClient.SendCommand<ResultCommandAvailableNetworks>
                (CommandsEnum.GetAvailableNetworks, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultGetAvailableNetworks;
        }

        /// <summary>
        /// Order the device to connect to specified network
        /// </summary>
        /// <param name="requestConnectToNetwork"></param>
        /// <returns></returns>
        public async Task<ResultCommandNetworkStatus> ConnectToClientNetwork(RequestCommandConnectToNetwork requestConnectToNetwork)
        {
            var resultConnectToNetwork = await _streamClient.SendCommand<RequestCommandConnectToNetwork, ResultCommandNetworkStatus>
                (CommandsEnum.ConnectToNetwork, requestConnectToNetwork, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultConnectToNetwork;
        }

        /// <summary>
        /// Order the device to disconnected from specified network
        /// </summary>
        /// <param name="requestDisconnectFromNetwork"></param>
        /// <returns></returns>
        public async Task<ResultCommandNetworkStatus> DisconnectFromClientNetwork(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork)
        {
            var resultDisconnectFromNetwork = await _streamClient.SendCommand<RequestCommandDisconnectFromNetwork, ResultCommandNetworkStatus>
                (CommandsEnum.DisconnectFromNetwork, requestDisconnectFromNetwork, Common.Helpers.SharedConstants.DEFAULT_ENCRYPTION_KEY);
            return resultDisconnectFromNetwork;
        }
    }
}
