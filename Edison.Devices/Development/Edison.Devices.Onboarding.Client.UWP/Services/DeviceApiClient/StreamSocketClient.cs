using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Common.Helpers;
using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Edison.Devices.Onboarding.Client.UWP
{
    public class StreamSocketClient : IStreamClient
    {
        private SemaphoreSlim _SocketLock = new SemaphoreSlim(1, 1);
        private readonly HostName _hostname;
        private readonly string _port;
        private StreamSocket _socket;
        private DataWriter _writer;
        private DataReader _reader;

        public StreamSocketClient(string hostname, int port)
        {
            _hostname = new HostName(hostname);
            _port = port.ToString();
        }

        public async Task<U> SendCommand<T, U>(CommandsEnum requestCommandType, T parameters, string passkey) where T : RequestCommand where U : ResultCommand, new()
        {
            Command requestCommand = new Command()
            {
                BaseCommand = requestCommandType,
                Data = JsonConvert.SerializeObject(parameters)
            };

            return await SendCommand<U>(requestCommand, passkey);
        }

        public async Task<T> SendCommand<T>(CommandsEnum requestCommandType, string passkey) where T : ResultCommand, new()
        {
            Command requestCommand = new Command()
            {
                BaseCommand = requestCommandType,
                Data = null
            };

            return await SendCommand<T>(requestCommand, passkey);
        }

        private async Task<T> SendCommand<T>(Command requestCommand, string passkey) where T : ResultCommand, new()
        {
            await _SocketLock.WaitAsync();
            string errorMessage = null;
            try
            {
                if (await Connect(_hostname, _port))
                {
                    if (await SendRequest(requestCommand, passkey))
                    {
                        Command response = await ReadResponse(passkey);
                        if (response != null)
                        {
                            if (response.BaseCommand == requestCommand.BaseCommand + 100)
                            {
                                Close();
                                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(response.Data)); //todo: probably broken after command data changed from string to object 1/11/18
                            }
                            else
                            {
                                errorMessage =$"SendCommand: The response command should be {requestCommand.BaseCommand + 100} but instead is {response.BaseCommand}.";
                            }
                        }
                        else
                        {
                            errorMessage = $"SendCommand: The response could not be read.";
                        }
                    }
                    else
                    {
                        errorMessage = $"SendCommand: The command could not be sent.";
                    }
                    Close();
                }
                else
                {
                    errorMessage = $"SendCommand: The connection failed.";
                }
            }
            catch(Exception e)
            {
                errorMessage = $"SendCommand: Unexpected error: {e.Message}";
            }
            finally
            {
                _SocketLock.Release();
            }

            //Return Error result
            Debug.WriteLine(errorMessage);
            return new T
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        private async Task<bool> Connect(HostName hostName, string connectionPortString)
        {
            try
            {
                Debug.WriteLine($"Opening socket: {hostName}:{connectionPortString}");
                _socket = new StreamSocket();
                await _socket.ConnectAsync(hostName, connectionPortString, SocketProtectionLevel.PlainSocket);
                _writer = new DataWriter(_socket.OutputStream);
                _reader = new DataReader(_socket.InputStream);
                Debug.WriteLine($"Socket connected: {hostName}:{connectionPortString}");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SendRequest(Command command, string passkey)
        {
            string requestData = CommandParserHelper.SerializeAndEncryptCommand(command, passkey);

            try
            {
                //First byte is the size of the message
                _writer.WriteUInt32(_writer.MeasureString(requestData));
                //Next is the actual message
                _writer.WriteString(requestData);
                //Send the message
                await _writer.StoreAsync();
                await _writer.FlushAsync();
                //Log
                Debug.WriteLine($"Sent: {command.BaseCommand}, {command.Data}");
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"SendRequest: {e.Message}");
                return false;
            }
        }

        private async Task<Command> ReadResponse(string passkey)
        {
            try
            {
                while (true)
                {
                    //Get message size length
                    uint uintSizeLoad = await _reader.LoadAsync(sizeof(uint));
                    if (uintSizeLoad != sizeof(uint))
                        return null;
                    uint responseSize = _reader.ReadUInt32();

                    //Load message
                    uint responseSizeLoad = await _reader.LoadAsync(responseSize);
                    if (responseSize != responseSizeLoad)
                        return null;

                    //Read message
                    string responseString = _reader.ReadString(responseSizeLoad);

                    Command response = CommandParserHelper.DeserializeAndDecryptCommand(responseString, passkey);
                    Debug.WriteLine($"Incoming: {response.BaseCommand}, {response.Data}");
                    return response;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine($"ReadResponse: {e.Message}");
                return null;
            }
        }

        private void Close()
        {
            Debug.WriteLine($"Closing socket");
            _writer.DetachStream();
            _writer.Dispose();

            _reader.DetachStream();
            _reader.Dispose();

            _socket.Dispose();
            Debug.WriteLine($"Closed socket");
        }
    }
}
