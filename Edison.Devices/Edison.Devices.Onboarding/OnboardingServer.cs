using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Services;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Models;

namespace Edison.Devices.Onboarding
{
    internal class OnboardingServer
    {
        private StreamSockerServer _StreamSockerHandler = null;
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
            if (!await StartAPApp())
                return;

            //Start Stream Socker Service
            _StreamSockerHandler = new StreamSockerServer();
            _StreamSockerHandler.CommandReceived += CommandReceived;
            await _StreamSockerHandler.Start();

            //Wait for AP Timeout
            _LastAccess = DateTime.UtcNow;
            while (true)
            {
                if (_APTimeoutEnabled && DateTime.UtcNow.AddMinutes(-(APTimeoutMinutes)) > _LastAccess)
                {
                    await StopAPApp();
                    break;
                }
                await Task.Delay(500);
            }
        }

        private async Task<bool> StartAPApp()
        {
            DebugHelper.LogInformation("Starting IoTOnboardingTask...");
            var resultStart = await _PortalService.StartApp("IoTOnboardingTask");
            if (!resultStart.IsSuccess)
            {
                DebugHelper.LogCritical("The application IoTOnboardingTask could not be started.");
                return false;
            }
            DebugHelper.LogInformation("IoTOnboardingTask started");
            return true;
        }

        private async Task StopAPApp()
        {
            DebugHelper.LogInformation($"Onboarding has been on for {APTimeoutMinutes} minutes without new commands. Stopping IoTOnboardingTask...");
            //Stopping endpoint
            var resultStop = await _PortalService.StopApp("IoTOnboardingTask");
            if (!resultStop.IsSuccess)
                DebugHelper.LogError("The application IoTOnboardingTask could not be stopped.");
            else
                DebugHelper.LogInformation("IoTOnboardingTask stopped");
        }

        private bool InitializePortalAPI()
        {
            try
            {
                PortalApiHelper.Init("Administrator", SecretManager.PortalPassword);
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
                    case CommandsEnum.GenerateCSR:
                        ProcessCommand(commandArgs, () => { return _ProvisioningService.GenerateCSR(); });
                        break;
                    case CommandsEnum.ListFirmwares:
                        await ProcessCommand(commandArgs, async () => { return await _PortalService.ListFirmwares(); });
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
                    case CommandsEnum.Unknown:
                        throw new Exception($"Command ${commandArgs.InputCommand.BaseCommand} not found.");
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogCritical(e.Message);
                DebugHelper.LogCritical(e.StackTrace);
            }
        }

        private ResultCommand SetAPTimeoutValue(bool state)
        {
            _APTimeoutEnabled = state;
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
    }
}
