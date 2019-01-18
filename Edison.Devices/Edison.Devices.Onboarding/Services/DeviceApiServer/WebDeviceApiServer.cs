using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using Edison.Devices.Onboarding.Helpers;
using Edison.Devices.Onboarding.Interfaces;
using Edison.Devices.Onboarding.Models;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Services
{
    internal class WebDeviceApiServer : BaseDeviceApiServer, IDeviceApiServer
    {
        public event Func<CommandEventArgs, Task> CommandReceived;
        private const uint BufferSize = 16384;
        private const string EdisonEncryptHeader = "edison-encrypted";
        private HttpListener _listener = new HttpListener();

        /// <summary>
        /// Start the server
        /// </summary>
        /// <returns></returns>
        public async void Start()
        {
            DebugHelper.LogInformation($"WebServer starting on port {SharedConstants.DEVICE_API_PORT}");

            var network = await WaitForAPInterface();

            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{SharedConstants.DEVICE_API_PORT}/");
            _listener.Start();

            DebugHelper.LogInformation($"WebServer: Listening for connection on {SharedConstants.DEVICE_API_PORT}");
            bool retry = false;
            while (true)
            {
                // Enter the listening loop.
                try
                {
                    if (_listener == null)
                        break;

                    HttpListenerContext context = await _listener.GetContextAsync();
                    await ProcessConnectionReceived(context);
                    retry = false;

                    if (_listener == null)
                        break;
                }
                catch (Exception e)
                {
                    DebugHelper.LogError($"WebServer: Error handling request: {e.Message}");
                    if (retry) //Errors keep happening. Lets try to restart the server
                        break;
                    retry = true;
                }
            }
            DebugHelper.LogInformation($"WebServer: Stopping...");
            Stop();
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            try
            {
                if(_listener != null)
                {
                    _listener.Stop();
                    _listener.Close();
                    _listener = null;
                }
            }
            catch(Exception e)
            {
                DebugHelper.LogError($"WebServer Error during closing: {e.Message}.");
            }
        }

        /// <summary>
        /// New API Request
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async Task ProcessConnectionReceived(HttpListenerContext context)
        {
            DebugHelper.LogInformation($"WebServer: Connection received from {context.Request.RemoteEndPoint}");

            try
            {
                if (context.Request.Url.AbsolutePath == "/")
                {
                    SendResponse("Onboarding App is running", context.Response);
                }
                else
                {

                    Tuple<Command, HttpStatusCode> commandAndStatus = GetCommandFromRequest(context.Request);
                    Command command = commandAndStatus.Item1;
                    if (command == null)
                    {
                        SendResponse(Command.CreateErrorCommand($"The command was null."), context.Response, false, HttpStatusCode.BadRequest);
                        return;
                    }
                    if (command.BaseCommand == CommandsEnum.ResultError)
                    {
                        SendResponse(command, context.Response, false, commandAndStatus.Item2);
                        return;
                    }

                    //Send command
                    CommandEventArgs commandArgs = new CommandEventArgs() { InputCommand = command };
                    await CommandReceived?.Invoke(commandArgs);

                    //If output command retrieve, send it
                    if (commandArgs.OutputCommand != null)
                        SendResponse(commandArgs.OutputCommand, context.Response, SecretManager.IsEncryptionEnabled);
                }
            }
            catch (Exception loopException)
            {
                SendResponse(Command.CreateErrorCommand($"Error handling request: {loopException.Message}"), context.Response, false, HttpStatusCode.InternalServerError);
            }

            DebugHelper.LogInformation($"WebServer: Connection from {context.Request.RemoteEndPoint} ended.");

            context.Response.OutputStream.Flush();
            context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Send Response
        /// </summary>
        /// <param name="command"></param>
        /// <param name="writer"></param>
        /// <param name="encrypt"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private void SendResponse(Command command, HttpListenerResponse response, bool encrypt, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var json = CommandParserHelper.SerializeCommand(command);

            //Headers
            response.StatusCode = (int)statusCode;
            response.ContentType = "application/json";
            response.ContentLength64 = json.Length;
            //Encrypt
            if (encrypt)
            {
                json = CommandParserHelper.EncryptMessage(json, SecretManager.EncryptionKey);
                response.AddHeader(EdisonEncryptHeader, "true");
            }
            response.AddHeader("Connection", "close");

            response.OutputStream.Write(Encoding.UTF8.GetBytes(json));
            DebugHelper.LogInformation($"Sent: {command.BaseCommand}, {command.Data}");
        }

        /// <summary>
        /// Send Response
        /// </summary>
        /// <param name="command"></param>
        /// <param name="writer"></param>
        /// <param name="encrypt"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        private void SendResponse(string message, HttpListenerResponse response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            //Headers
            response.ContentLength64 = message.Length;
            response.ContentType = "application/json";
            response.StatusCode = (int)statusCode;

            response.OutputStream.Write(Encoding.UTF8.GetBytes(message));
            DebugHelper.LogInformation($"Sent message: '{message}'");
        }

        /// <summary>
        /// Performs a series of check to ensure the call is legal, and return a Command object
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        private Tuple<Command, HttpStatusCode> GetCommandFromRequest(HttpListenerRequest httpRequest)
        {
            //Check the authentication
            if (httpRequest.Headers["authorization"] == null || httpRequest.Headers["authorization"] != SecretManager.PortalPasswordBase64)
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand("Unauthorized."), HttpStatusCode.Unauthorized);

            //if Encryption is enabled, the device will expect a request with encrypted header
            if (SecretManager.IsEncryptionEnabled && httpRequest.Headers[EdisonEncryptHeader] == null)
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand("Encryption is enabled on the device."), HttpStatusCode.Unauthorized);

            //Listen to /edison/ endpoints only
            string apiPath = httpRequest.Url.AbsolutePath.ToLower();
            if (!apiPath.StartsWith("/edison/"))
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand("Not found."), HttpStatusCode.NotFound);

            //Retrieve the command from the endpoint
            string commandString = apiPath.Replace("/edison/", "");
            if (!Enum.TryParse(typeof(CommandsEnum), commandString, true, out object commandResult))
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand($"The command '{commandString}' was not found."), HttpStatusCode.NotFound);

            //Body
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(httpRequest.InputStream))
            {
                body = reader.ReadToEnd();
            }

            //Decrypt the message
            if (httpRequest.Headers[EdisonEncryptHeader] != null)
                body = CommandParserHelper.DecryptMessage(body, SecretManager.EncryptionKey);

            //Generate the command object
            CommandsEnum command = (CommandsEnum)commandResult;
            if (httpRequest.HttpMethod == "GET" && commandString.StartsWith("get"))
                return new Tuple<Command, HttpStatusCode>(new Command() { BaseCommand = command }, HttpStatusCode.OK);
            else if (httpRequest.HttpMethod == "POST" && !commandString.StartsWith("get"))
            {
                //If the request is POST, ensure that the content-type header is application/json
                if (httpRequest.Headers["content-type"] != null && httpRequest.Headers["content-type"] == "application/json")
                    return new Tuple<Command, HttpStatusCode>(new Command() { BaseCommand = command, Data = body }, HttpStatusCode.OK);
                return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand($"The body content must be a json string."), HttpStatusCode.BadRequest);
            }

            //Method is not GET or POST, return unsupported error
            return new Tuple<Command, HttpStatusCode>(Command.CreateErrorCommand($"Method {httpRequest.HttpMethod} {command} not supported."), HttpStatusCode.BadRequest);
        }
    }
}
