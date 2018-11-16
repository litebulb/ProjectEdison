using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Edison.Devices.Onboarding.Models;
using Windows.Devices.WiFi;
using System.Net.NetworkInformation;
using System.Net;
using Windows.Networking.Connectivity;

namespace Edison.Devices.Onboarding.Services
{
    internal class StreamSockerServer
    {
        private const uint BufferSize = 8192;
        public event Func<CommandEventArgs, Task> CommandReceived;


        public async Task Start()
        {
            DebugHelper.LogInformation($"StreamSocker Server starting on port {SharedConstants.CONNECTION_PORT}");

            NetworkInterface networkAP = GetAPNetworkInterface();
            while(networkAP == null || networkAP.OperationalStatus != OperationalStatus.Up)
            {
                await Task.Delay(1000);
                DebugHelper.LogVerbose($"Waiting for AP network to be up...");
                networkAP = GetAPNetworkInterface();
            }

            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += ConnectionReceived;
            await listener.BindServiceNameAsync(SharedConstants.CONNECTION_PORT.ToString());

            DebugHelper.LogInformation($"Listening for StreamSocket connection on {SharedConstants.CONNECTION_PORT}");
        }

        private NetworkInterface GetAPNetworkInterface()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                                ip.Address.ToString() == SharedConstants.SOFT_AP_IP)
                            {
                                return ni;
                            }
                        }
                    }
                }
            } catch(Exception e)
            {
                DebugHelper.LogError($"GetAPNetworkInterface: {e.Message}");
                DebugHelper.LogError($"GetAPNetworkInterface: {e.StackTrace}");
            }
            return null;
        }

        private async Task SendResponse(Command response, DataWriter writer, string passkey)
        {
            using (var stream = new MemoryStream())
            {
                // Send request
                string requestData = CommandParserHelper.SerializeAndEncryptCommand(response, passkey);
                writer.WriteUInt32(writer.MeasureString(requestData));
                writer.WriteString(requestData);
                await writer.StoreAsync();
                DebugHelper.LogInformation($"Sent: {response.BaseCommand}, {response.Data}");
            }
        }

        private async Task<Command> ReceiveCommand(DataReader reader, string passkey)
        {
            //Get message size length
            uint uintSizeLoad = await reader.LoadAsync(sizeof(uint));
            if (uintSizeLoad != sizeof(uint))
                return null;
            uint responseSize = reader.ReadUInt32();

            //Load message
            uint responseSizeLoad = await reader.LoadAsync(responseSize);
            if (responseSize != responseSizeLoad)
                return null;

            //Read message
            string responseString = reader.ReadString(responseSizeLoad);

            if (responseString.Length != 0)
            {
                Command msg = CommandParserHelper.DeserializeAndDecryptCommand(responseString, passkey);
                DebugHelper.LogInformation($"Incoming: {msg.BaseCommand}, {msg.Data}");
                return msg;
            }
            return null;
        }

        private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DebugHelper.LogInformation($"Connection received from {args.Socket.Information.RemoteAddress.DisplayName}:{args.Socket.Information.RemotePort}");

            string passkey = SecretManager.SocketPassphrase; //Passkey used for this session.
            try
            {
                using (var reader = new DataReader(args.Socket.InputStream))
                {
                    using (var writer = new DataWriter(args.Socket.OutputStream))
                    {
                        reader.InputStreamOptions = InputStreamOptions.Partial;
                        try
                        {
                            var networkCommand = await ReceiveCommand(reader, passkey);
                            //if (networkCommand == null) break;
                            if (networkCommand == null)
                            {
                                await SendResponse(Command.CreateErrorCommand($"The command was null."), writer, passkey);
                                return;
                            }

                            CommandEventArgs commandArgs = new CommandEventArgs()
                            {
                                InputCommand = networkCommand

                            };

                            //Send command
                            await CommandReceived?.Invoke(commandArgs);

                            //If output command retrieve, send it
                            if (commandArgs.OutputCommand != null)
                                await SendResponse(commandArgs.OutputCommand, writer, passkey);

                                
                        }
                        catch (Exception loopException)
                        {
                            await SendResponse(Command.CreateErrorCommand($"Error handling request: {loopException.Message}"), writer, passkey);
                        }
                        DebugHelper.LogInformation($"Connection from {args.Socket.Information.RemoteAddress.DisplayName}:{args.Socket.Information.RemotePort} ended");
                        if (args.Socket != null)
                        {
                            args.Socket.Dispose();
                            DebugHelper.LogVerbose("Socket disposed.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error handling request: {e.Message}");
            }
        }
    }
}
