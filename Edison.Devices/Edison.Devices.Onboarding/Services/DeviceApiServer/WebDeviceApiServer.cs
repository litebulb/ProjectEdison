using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Interfaces;
using Edison.Devices.Onboarding.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Edison.Devices.Onboarding.Services
{
    internal class WebDeviceApiServer : BaseDeviceApiServer, IDeviceApiServer
    {
        public event Func<CommandEventArgs, Task> CommandReceived;
        private const uint BufferSize = 16384;
        private const string EdisonEncryptHeader = "edison-encrypted";

        public async Task Start()
        {
            DebugHelper.LogInformation($"WebServer starting on port {SharedConstants.DEVICE_API_PORT}");

            var network = await WaitForAPInterface();

            var listener = new StreamSocketListener();
            await listener.BindServiceNameAsync(SharedConstants.DEVICE_API_PORT.ToString());
            listener.ConnectionReceived += ConnectionReceived;

            DebugHelper.LogInformation($"Listening for WebServer connection on {SharedConstants.DEVICE_API_PORT}");
        }

        /// <summary>
        /// New API Request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DebugHelper.LogInformation($"Connection received from {args.Socket.Information.RemoteAddress.DisplayName}:{args.Socket.Information.RemotePort}");

            try
            {
                using (var reader = new DataReader(args.Socket.InputStream))
                {
                    using (var writer = new DataWriter(args.Socket.OutputStream))
                    {
                        reader.InputStreamOptions = InputStreamOptions.Partial;
                        try
                        {
                            Tuple<Command, HttpStatusCode> commandAndStatus = await ReceiveCommand(reader);
                            Command command = commandAndStatus.Item1;
                            if (command == null)
                            {
                                await SendResponse(Command.CreateErrorCommand($"The command was null."), writer, false, HttpStatusCode.BadRequest);
                                return;
                            }
                            if (command.BaseCommand == CommandsEnum.ResultError)
                            {
                                await SendResponse(command, writer, false, commandAndStatus.Item2);
                                return;
                            }

                            //Send command
                            CommandEventArgs commandArgs = new CommandEventArgs() { InputCommand = command };
                            await CommandReceived?.Invoke(commandArgs);

                            //If output command retrieve, send it
                            if (commandArgs.OutputCommand != null)
                                await SendResponse(commandArgs.OutputCommand, writer, SecretManager.IsEncryptionEnabled);


                        }
                        catch (Exception loopException)
                        {
                            await SendResponse(Command.CreateErrorCommand($"Error handling request: {loopException.Message}"), writer, false, HttpStatusCode.InternalServerError);
                        }

                        DebugHelper.LogInformation($"Connection from {args.Socket.Information.RemoteAddress.DisplayName}:{args.Socket.Information.RemotePort} ended");
                    }
                }
            }
            catch (Exception e)
            {
                DebugHelper.LogError($"Error handling request: {e.Message}");
            }
        }

        /// <summary>
        /// Send Response
        /// </summary>
        /// <param name="command"></param>
        /// <param name="writer"></param>
        /// <param name="encrypt"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private async Task SendResponse(Command command, DataWriter writer, bool encrypt, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var json = CommandParserHelper.SerializeCommand(command);

            string encryptHeader = string.Empty;
            if (encrypt)
            {
                json = CommandParserHelper.EncryptMessage(json, SecretManager.EncryptionKey);
                encryptHeader = $"{EdisonEncryptHeader}: true\r\n";
            }

            var message = $"HTTP/1.1 {(int)statusCode} {statusCode}\r\nContent-Length: {json.Length}\r\ncontent-type: application/json\r\n{encryptHeader}Connection: close\r\n\r\n" + json;

            writer.WriteString(message);
            await writer.StoreAsync();
            DebugHelper.LogInformation($"Sent: {command.BaseCommand}, {command.Data}");

        }

        /// <summary>
        /// Receive command
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private async Task<Tuple<Command, HttpStatusCode>> ReceiveCommand(DataReader reader)
        {
            HttpRequest httpRequest = null;
            await reader.LoadAsync(BufferSize);
            string request = string.Empty;
            while (reader.UnconsumedBufferLength > 0)
            {
                request += reader.ReadString(reader.UnconsumedBufferLength);

                //Test httpRequest, it can happen if the stream isn't fully loaded.
                if (reader.UnconsumedBufferLength == 0)
                {
                    httpRequest = HttpRequest.FromString(request);
                    if (!httpRequest.ValidateHttpRequest())
                        await LoadAsyncWithTimeout(reader);
                }
            }

            //Process request
            httpRequest = HttpRequest.FromString(request);
            if (!httpRequest.ValidateHttpRequest())
                throw new Exception("HttpRequest: Content-Length does not match the size of body.");

            Tuple<Command, HttpStatusCode> command = GetCommandFromRequest(httpRequest);
            DebugHelper.LogInformation($"Incoming: {command.Item1.BaseCommand}, {command.Item1.Data}");
            return command;
        }

        /// <summary>
        /// Performs a series of check to ensure the call is legal, and return a Command object
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        private Tuple<Command, HttpStatusCode> GetCommandFromRequest(HttpRequest httpRequest)
        {
            //Check the authentication
            if (!httpRequest.Headers.ContainsKey("authorization") || httpRequest.Headers["authorization"] != SecretManager.PortalPasswordBase64)
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand("Unauthorized."), HttpStatusCode.Unauthorized);

            //if Encryption is enabled, the device will expect a request with encrypted header
            if (SecretManager.IsEncryptionEnabled && !httpRequest.Headers.ContainsKey(EdisonEncryptHeader))
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand("Encryption is enabled on the device."), HttpStatusCode.Unauthorized);

            //Listen to /edison/ endpoints only
            if (!httpRequest.Query.ToLower().StartsWith("/edison/"))
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand("Not found."), HttpStatusCode.NotFound);

            //Retrieve the command from the endpoint
            string commandString = httpRequest.Query.Replace("/edison/", "");
            if (!Enum.TryParse(typeof(CommandsEnum), commandString, true, out object commandResult))
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand($"The command '{commandString}' was not found."), HttpStatusCode.NotFound);

            //Decrypt the message
            if (httpRequest.Headers.ContainsKey(EdisonEncryptHeader))
                httpRequest.Body = CommandParserHelper.DecryptMessage(httpRequest.Body, SecretManager.EncryptionKey);

            //Generate the command object
            CommandsEnum command = (CommandsEnum)commandResult;
            if (httpRequest.Method == "GET" && commandString.StartsWith("get"))
                return new Tuple<Command, HttpStatusCode>(new Command() { BaseCommand = command }, HttpStatusCode.OK);
            else if (httpRequest.Method == "POST" && !commandString.StartsWith("get"))
            {
                //If the request is POST, ensure that the content-type header is application/json
                if (httpRequest.Headers.ContainsKey("content-type") && httpRequest.Headers["content-type"] == "application/json")
                    return new Tuple<Command, HttpStatusCode>(new Command() { BaseCommand = command, Data = httpRequest.Body }, HttpStatusCode.OK);
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand($"The body content must be a json string."), HttpStatusCode.BadRequest);
            }

            //Method is not GET or POST, return unsupported error
            return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand($"Method {httpRequest.Method} {command} not supported."), HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Perform an async load of the streamsocketlistener, with a timeout to avoid deadlock
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private async Task LoadAsyncWithTimeout(DataReader reader)
        {
            try
            {
                var timeoutSource = new CancellationTokenSource(1000);
                await reader.LoadAsync(BufferSize).AsTask(timeoutSource.Token);
            }
            catch (TaskCanceledException)
            {
                // Can potentially happen. It's here to avoid deadlocks.
            }
        }
    }
}
