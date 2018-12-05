using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Models;
using Edison.Devices.Onboarding.Interfaces;

namespace Edison.Devices.Onboarding
{
    internal class OnboardingServer
    {
        private IDeviceApiServer _DeviceApiServer = null;
        private WifiService _WifiService = null; 
        private ProvisioningService _ProvisioningService = null;
        private PortalService _PortalService = null;
        private DateTime _LastAccess = DateTime.UtcNow;
        private const int APTimeoutMinutes = 10;
        private bool _APTimeoutEnabled = true;

        public async Task Run()
        {
            DebugHelper.LogInformation("Onboarding starting");

            //Init Portal Api and turn on access point
            if (!InitializePortalAPI())
                return;

            //Start Services
            _WifiService = new WifiService();
            _ProvisioningService = new ProvisioningService();
            _PortalService = new PortalService();

            //Start AP App
            AccessPointHelper.StartAccessPoint();

            //Start Stream Socker Service
            _DeviceApiServer = new WebDeviceApiServer();
            _DeviceApiServer.CommandReceived += CommandReceived;
            await _DeviceApiServer.Start();

            //Wait for AP Timeout
            _LastAccess = DateTime.UtcNow;
            while (true)
            {
                if (_APTimeoutEnabled && SimulatedDevice.IsProvisioned && 
                    DateTime.UtcNow.AddMinutes(-(APTimeoutMinutes)) > _LastAccess)
                {
                    AccessPointHelper.StopAccessPoint();
                    break;
                }
                await Task.Delay(500);
            }
        }

        private bool InitializePortalAPI()
        {
            try
            {
                PortalApiHelper.Init("Administrator", SecretManager.PortalPassword);
                SecretManager.PortalPassword = string.IsNullOrEmpty(SecretManager.PortalPassword) ? SharedConstants.DEFAULT_PORTAL_PASSWORD : SecretManager.PortalPassword;
            }
            catch(Exception e)
            {
                DebugHelper.LogError($"The Portal API Access could not be initialized: {e.Message}");

                //Attempting default password
                if(SecretManager.PortalPassword != SharedConstants.DEFAULT_PORTAL_PASSWORD)
                {
                    try
                    {
                        PortalApiHelper.Init("Administrator", SharedConstants.DEFAULT_PORTAL_PASSWORD);
                        SecretManager.PortalPassword = SharedConstants.DEFAULT_PORTAL_PASSWORD;
                    }
                    catch (Exception e2)
                    {
                        DebugHelper.LogCritical($"The Portal API Access could not be initialized: {e2.Message}");
                        DebugHelper.LogCritical($"Cannot access the Portal API. Please flash a new firmware.");
                        return false;
                    }
                }
            }
            return true;
        }

        private async Task CommandReceived(CommandEventArgs commandArgs)
        {
            try
            {
                _LastAccess = DateTime.UtcNow; //Refreshing time for Access Point
                switch (commandArgs.InputCommand.BaseCommand)
                {
                    case CommandsEnum.GetAvailableNetworks:
                        await ProcessCommand(commandArgs, async () => {
                            return await _WifiService.GetAvailableNetworkListHandler(); });
                        break;
                    case CommandsEnum.ConnectToNetwork:
                        await ProcessCommand(commandArgs, async (RequestCommandConnectToNetwork request) => {
                            return await _WifiService.ConnectToNetworkHandler(request); });
                        break;
                    case CommandsEnum.DisconnectFromNetwork:
                        await ProcessCommand(commandArgs, async (RequestCommandDisconnectFromNetwork request) => {
                            return await _WifiService.DisconnectFromNetworkHandler(request);
                        });
                        break;
                    case CommandsEnum.GetDeviceId:
                        ProcessCommand(commandArgs, () => { return _ProvisioningService.GetDeviceId(); });
                        break;
                    case CommandsEnum.ProvisionDevice:
                        await ProcessCommand(commandArgs, async (RequestCommandProvisionDevice request) => {
                            return await _ProvisioningService.ProvisionDevice(request, SecretManager.CertificatePasskey);
                        });
                        break;
                    case CommandsEnum.GetGeneratedCSR:
                        ProcessCommand(commandArgs, () => { return _ProvisioningService.GenerateCSR(); });
                        break;
                    case CommandsEnum.GetFirmwares:
                        await ProcessCommand(commandArgs, async () => { return await _PortalService.GetFirmwares(); });
                        break;
                    case CommandsEnum.SetDeviceType:
                        await ProcessCommand(commandArgs, async (RequestCommandSetDeviceType request) => {
                            return await _PortalService.SetDeviceType(request);
                        });
                        break;
                    case CommandsEnum.SetDeviceName:
                        await ProcessCommand(commandArgs, async (RequestCommandSetDeviceName request) => {
                            return await _PortalService.SetDeviceName(request);
                        });
                        break;
                    case CommandsEnum.GetAccessPointSettings:
                        await ProcessCommand(commandArgs, async () => { return await _PortalService.GetAccessPointSettings(); });
                        break;
                    case CommandsEnum.SetDeviceSecretKeys:
                        await ProcessCommand(commandArgs, async (RequestCommandSetDeviceSecretKeys request) => {
                            return await _PortalService.SetDeviceSecretKeys(request);
                        });
                        break;
                    case CommandsEnum.DisableAccessPointTimeout:
                        ProcessCommand(commandArgs, () => { return SetAPTimeoutValue(false); });
                        break;
                    case CommandsEnum.EnableAccessPointTimeout:
                        ProcessCommand(commandArgs, () => { return SetAPTimeoutValue(true); });
                        break;
                    case CommandsEnum.EnableEncryption:
                        ProcessCommand(commandArgs, () => { return SetEncryptionState(true); });
                        break;
                    case CommandsEnum.DisableEncryption:
                        ProcessCommand(commandArgs, () => { return SetEncryptionState(false); });
                        break;
                    case CommandsEnum.GetEncryptionState:
                        ProcessCommand(commandArgs, () => { return GetEncryptionStatus(); });
                        break;
                    case CommandsEnum.Unknown:
                        throw new Exception($"Command ${commandArgs.InputCommand.BaseCommand} not found.");
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogCritical(e.Message);
                DebugHelper.LogCritical(e.StackTrace);
                ProcessCommand(commandArgs, () => { return ProcessError(e); });
            }
        }

        #region Command Processing
        private ResultCommand ProcessError(Exception e)
        {
            return ResultCommand.CreateFailedCommand($"Error processing the command: {e.Message}");
        }

        private ResultCommandEncryptionStatus GetEncryptionStatus()
        {
            return new ResultCommandEncryptionStatus() { Enabled = SecretManager.IsEncryptionEnabled, IsSuccess = true };
        }

        private ResultCommand SetAPTimeoutValue(bool state)
        {
            _APTimeoutEnabled = state;
            return ResultCommand.CreateSuccessCommand();
        }

        private ResultCommand SetEncryptionState(bool state)
        {
            SecretManager.IsEncryptionEnabled = state;
            return ResultCommand.CreateSuccessCommand();
        }

        private void ProcessCommand(CommandEventArgs commandArgs, Func<ResultCommand> commandProcess)
        {
            ResultCommand resultCommand = commandProcess();
            commandArgs.OutputCommand = new Command()
            {
                BaseCommand = commandArgs.InputCommand.BaseCommand + 100,
                Data = resultCommand != null ? JsonConvert.SerializeObject(resultCommand) : null
            };
        }

        private async Task ProcessCommand(CommandEventArgs commandArgs, Func<Task<ResultCommand>> commandProcess) 
        {
            ResultCommand resultCommand = await commandProcess();
            commandArgs.OutputCommand = new Command()
            {
                BaseCommand = commandArgs.InputCommand.BaseCommand + 100,
                Data = resultCommand != null ? JsonConvert.SerializeObject(resultCommand) : null
            };
        }

        private async Task ProcessCommand<T>(CommandEventArgs commandArgs, Func<T,Task<ResultCommand>> commandProcess) where T : RequestCommand
        {
            T requestCommand = null;
            ResultCommand resultCommand = null;
            if (!string.IsNullOrEmpty(commandArgs.InputCommand.Data))
                requestCommand = JsonConvert.DeserializeObject<T>(commandArgs.InputCommand.Data);
            if(requestCommand != null)
                resultCommand = await commandProcess(requestCommand);
            commandArgs.OutputCommand = new Command()
            {
                BaseCommand = commandArgs.InputCommand.BaseCommand + 100,
                Data = resultCommand != null ? JsonConvert.SerializeObject(resultCommand) : null
            };
        }
        #endregion
    }
}
