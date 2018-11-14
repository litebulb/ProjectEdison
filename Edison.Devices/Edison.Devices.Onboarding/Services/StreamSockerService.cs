using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Common.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Edison.Devices.Onboarding.Services
{
    internal class StreamSockerService
    {
        public event Func<CommandEventArgs, Task> CommandReceived;

        public StreamSockerService()
        {

        }

        public async Task Start()
        {
            DebugHelper.LogInformation("StreamSocker Server starting");

            var Listener = new StreamSocketListener();
            Listener.ConnectionReceived += Listener_ConnectionReceived;
            string connectionString = "50074";
            await Listener.BindServiceNameAsync(connectionString);

            DebugHelper.LogInformation($"Listening for StreamSocket connection on {connectionString}");
        }

        private async Task SendResponse(Command response, DataWriter writer)
        {
            using (var stream = new MemoryStream())
            {
                // Send request
                string requestData = CommandParserHelper.SerializeAndEncryptCommand(response);
                writer.WriteString(requestData);
                await writer.StoreAsync();
                DebugHelper.LogInformation($"Sent: {response.BaseCommand}, {response.Data}");
            }

        }

        private async Task<Command> GetNextRequest(DataReader reader)
        {
            await reader.LoadAsync(8192);

            string data = string.Empty;
            while (reader.UnconsumedBufferLength > 0)
            {
                data += reader.ReadString(reader.UnconsumedBufferLength);
            }

            if (data.Length != 0)
            {
                Command msg = CommandParserHelper.DeserializeAndDecryptCommand(data);
                DebugHelper.LogInformation($"Incoming: {msg.BaseCommand}, {msg.Data}");
                return msg;
            }
            return null;
        }

        private async Task HandleSocket(IInputStream istream, IOutputStream ostream)
        {
            try
            {
                using (var reader = new DataReader(istream))
                {
                    using (var writer = new DataWriter(ostream))
                    {
                        reader.InputStreamOptions = InputStreamOptions.Partial;
                        while (true)
                        {
                            var networkCommand = await GetNextRequest(reader);
                            if (networkCommand == null) break;

                            CommandEventArgs commandArgs = new CommandEventArgs()
                            {
                                InputCommand = networkCommand

                            };

                            //Send command
                            await CommandReceived?.Invoke(commandArgs);

                            //If output command retrieve, send it
                            if (commandArgs.OutputCommand != null)
                                await SendResponse(commandArgs.OutputCommand, writer);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogWarning($"HandleSocket: {e.Message}");
            }
        }

        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DebugHelper.LogInformation($"Connection received from {args.Socket.Information.RemoteAddress.DisplayName}:{args.Socket.Information.RemotePort}");
            await HandleSocket(args.Socket.InputStream, args.Socket.OutputStream);
        }
    }
}
