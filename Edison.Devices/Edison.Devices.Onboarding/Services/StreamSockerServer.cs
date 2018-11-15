using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Common.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Edison.Devices.Onboarding.Models;

namespace Edison.Devices.Onboarding.Services
{
    internal class StreamSockerServer
    {
        private const uint BufferSize = 8192;
        private const uint ServerPort = 50074;
        public event Func<CommandEventArgs, Task> CommandReceived;

        public async Task Start()
        {
            DebugHelper.LogInformation($"StreamSocker Server starting on port {ServerPort}");

            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += ConnectionReceived;
            await listener.BindServiceNameAsync(ServerPort.ToString());

            DebugHelper.LogInformation($"Listening for StreamSocket connection on {ServerPort}");
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
                        while (true)
                        {
                            try
                            {
                                var networkCommand = await ReceiveCommand(reader, passkey);
                                if (networkCommand == null) break;

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
                            catch(Exception loopException)
                            {
                                await SendResponse(Command.CreateErrorCommand($"Error handling request: {loopException.Message}"), writer, passkey);
                            }
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
