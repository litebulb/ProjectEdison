using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Common.Models.CommandModels;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Helpers;

namespace Edison.Devices.Onboarding
{
    internal class OnboardingServer
    {
        private StreamSockerService _StreamSockerHandler = null;
        private WifiService _WifiService = null;
        private ProvisioningService _ProvisioningService = null;
        private PortalService _PortalService = null;

        public async Task Run()
        {
            DebugHelper.LogInformation("Onboarding starting");

            //Start Wifi Service
            _WifiService = new WifiService();

            //Start Provisioning Service
            _ProvisioningService = new ProvisioningService();

            //Start Portal Service
            _PortalService = new PortalService();

            //Start Stream Socker Service
            _StreamSockerHandler = new StreamSockerService();
            _StreamSockerHandler.CommandReceived += _StreamSockerHandler_CommandReceived;
            await _StreamSockerHandler.Start();

            DateTime time = DateTime.UtcNow;
            while (true)
            {
                if(time.AddMinutes(-1) > time)
                {
                    DebugHelper.LogVerbose("Onboarding is live");
                    time = DateTime.UtcNow;
                }
                await Task.Delay(500);
            }
        }

        private async Task _StreamSockerHandler_CommandReceived(CommandEventArgs commandArgs)
        {
            try
            {
                switch (commandArgs.InputCommand.BaseCommand)
                {
                    case CommandsEnum.GetAvailableNetworks:
                        ResultCommandAvailableNetworks availableNetworks = await _WifiService.GetAvailableNetworkListHandler();
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultGetAvailableNetworks,
                            Data = availableNetworks != null ? JsonConvert.SerializeObject(availableNetworks) : null
                        };
                        break;
                    case CommandsEnum.ConnectToNetwork:
                        NetworkInformation dataNetworkInformation = JsonConvert.DeserializeObject<NetworkInformation>(commandArgs.InputCommand.Data);
                        ResultCommandNetworkStatus networkStatusConnect = null;
                        if (dataNetworkInformation != null)
                            networkStatusConnect = await _WifiService.ConnectToNetworkHandler(dataNetworkInformation);
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultConnectToNetwork,
                            Data = networkStatusConnect != null ? JsonConvert.SerializeObject(networkStatusConnect) : null
                        };
                        break;
                    case CommandsEnum.DisconnectFromNetwork:
                        ResultCommandNetworkStatus networkStatusDisconnect = await _WifiService.DisconnectFromNetworkHandler(commandArgs.InputCommand.Data);
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultDisconnectFromNetwork,
                            Data = networkStatusDisconnect != null ? JsonConvert.SerializeObject(networkStatusDisconnect) : null
                        };
                        break;
                    case CommandsEnum.GetDeviceId:
                        ResultCommandGetDeviceId resultGetDeviceId = _ProvisioningService.GetDeviceId();
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultGetDeviceId,
                            Data = resultGetDeviceId != null ? JsonConvert.SerializeObject(resultGetDeviceId) : null
                        };
                        break;
                    case CommandsEnum.ProvisionDevice:
                        DeviceCertificateModel dataProvisionDevice = JsonConvert.DeserializeObject<DeviceCertificateModel>(commandArgs.InputCommand.Data);
                        ResultCommand resultProvisionDevice = null;
                        if(dataProvisionDevice != null)
                            resultProvisionDevice = await _ProvisioningService.ProvisionDevice(dataProvisionDevice, SharedConstants.CERTIFICATE_PASSKEY);
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultProvisionDevice,
                            Data = resultProvisionDevice != null ? JsonConvert.SerializeObject(resultProvisionDevice) : null
                        };
                        break;
                    case CommandsEnum.GenerateCSR:
                        ResultCommandGenerateCSR resultGenerateCSR = _ProvisioningService.GenerateCSR();
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultGenerateCSR,
                            Data = resultGenerateCSR != null ? JsonConvert.SerializeObject(resultGenerateCSR) : null
                        };
                        break;
                    case CommandsEnum.ListFirmwares:
                        ResultCommandListFirmwares resultListFirmwares = await _PortalService.ListFirmwares();
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultListFirmwares,
                            Data = resultListFirmwares != null ? JsonConvert.SerializeObject(resultListFirmwares) : null
                        };
                        break;
                    case CommandsEnum.SetDeviceType:
                        ResultCommand resultSetDeviceType = await _PortalService.SetDeviceType(commandArgs.InputCommand.Data);
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultSetDeviceType,
                            Data = resultSetDeviceType != null ? JsonConvert.SerializeObject(resultSetDeviceType) : null
                        };
                        break;
                    case CommandsEnum.GetAccessPointSettings:
                        ResultCommandSoftAPSettings resultGetAccessPointSettings = await _PortalService.GetAccessPointSettings();
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultGetAccessPointSettings,
                            Data = resultGetAccessPointSettings != null ? JsonConvert.SerializeObject(resultGetAccessPointSettings) : null
                        };
                        break;
                    case CommandsEnum.SetAccessPoint:
                        SoftAPSettings dataSetAccessPoint = JsonConvert.DeserializeObject<SoftAPSettings>(commandArgs.InputCommand.Data);
                        ResultCommand resultSetAccessPoint = null;
                        if (dataSetAccessPoint != null)
                            resultSetAccessPoint = await _PortalService.SetAccessPoint(dataSetAccessPoint);
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultSetAccessPoint,
                            Data = resultSetAccessPoint != null ? JsonConvert.SerializeObject(resultSetAccessPoint) : null
                        };
                        break;
                    case CommandsEnum.SetDevicePassword:
                        ChangeDevicePassword dataSetDevicePassword = JsonConvert.DeserializeObject<ChangeDevicePassword>(commandArgs.InputCommand.Data);
                        ResultCommand resultSetDevicePassword = null;
                        if (dataSetDevicePassword != null)
                            resultSetDevicePassword = await _PortalService.SetDevicePassword(dataSetDevicePassword);
                        commandArgs.OutputCommand = new Command()
                        {
                            BaseCommand = CommandsEnum.ResultSetDevicePassword,
                            Data = resultSetDevicePassword != null ? JsonConvert.SerializeObject(resultSetDevicePassword) : null
                        };
                        break;
                    case CommandsEnum.Unknown:
                        DebugHelper.LogError("Commands not found");
                        break;
                }
            }
            catch(Exception e)
            {
                DebugHelper.LogCritical(e.Message);
                DebugHelper.LogCritical(e.StackTrace);
            }
        }
    }
}
