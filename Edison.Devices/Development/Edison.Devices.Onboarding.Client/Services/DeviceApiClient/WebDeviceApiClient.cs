using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Devices.Onboarding.Client.Services
{
    public class WebDeviceApiClient : RestBaseService, IDeviceApiClient
    {
        public WebDeviceApiClient(string restServiceUrl, string token)
            : base(restServiceUrl, Convert.ToBase64String(Encoding.ASCII.GetBytes($"Administrator:{token}")), "Basic")
        {
        }

        public async Task<ResultCommandNetworkStatus> ConnectToClientNetwork(RequestCommandConnectToNetwork requestConnectToNetwork)
        {
            RestRequest request = await PrepareQuery("ConnectToNetwork", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestConnectToNetwork), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandNetworkStatus>(queryResult.Data);
            else
                Debug.WriteLine($"ConnectToNetwork: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"ConnectToNetwork: {queryResult.StatusCode}");
        }

        public async Task<ResultCommandNetworkStatus> DisconnectFromClientNetwork(RequestCommandDisconnectFromNetwork requestDisconnectFromNetwork)
        {
            RestRequest request = await PrepareQuery("DisconnectFromNetwork", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestDisconnectFromNetwork), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandNetworkStatus>(queryResult.Data);
            else
                Debug.WriteLine($"DisconnectFromNetwork: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandNetworkStatus>($"DisconnectFromNetwork: {queryResult.StatusCode}");
        }

        public async Task<ResultCommandGenerateCSR> GetGeneratedCSR()
        {
            RestRequest request = await PrepareQuery("GetGeneratedCSR", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandGenerateCSR>(queryResult.Data);
            else
                Debug.WriteLine($"GetGeneratedCSR: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandGenerateCSR>($"GetGeneratedCSR: {queryResult.StatusCode}");
        }

        public async Task<ResultCommandSoftAPSettings> GetAccessPointSettings()
        {
            RestRequest request = await PrepareQuery("GetAccessPointSettings", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandSoftAPSettings>(queryResult.Data);
            else
                Debug.WriteLine($"GetAccessPointSettings: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandSoftAPSettings>($"GetAccessPointSettings: {queryResult.StatusCode}");
        }

        public async Task<ResultCommandGetDeviceId> GetDeviceId()
        {
            RestRequest request = await PrepareQuery("GetDeviceId", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandGetDeviceId>(queryResult.Data);
            else
                Debug.WriteLine($"GetDeviceId: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandGetDeviceId>($"GetDeviceId: {queryResult.StatusCode}");
        }

        public async Task<ResultCommandListFirmwares> GetFirmwares()
        {
            RestRequest request = await PrepareQuery("GetFirmwares", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandListFirmwares>(queryResult.Data);
            else
                Debug.WriteLine($"GetFirmwares: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandListFirmwares>($"GetFirmwares: {queryResult.StatusCode}");
        }

        public async Task<ResultCommand> ProvisionDevice(RequestCommandProvisionDevice requestProvisionDevice)
        {
            RestRequest request = await PrepareQuery("ProvisionDevice", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestProvisionDevice), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommand>(queryResult.Data);
            else
                Debug.WriteLine($"ProvisionDevice: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommand>($"ProvisionDevice: {queryResult.StatusCode}");
        }

        public async Task<ResultCommand> SetDeviceSecretKeys(RequestCommandSetDeviceSecretKeys requestSetDeviceSecretKeys)
        {
            RestRequest request = await PrepareQuery("SetDeviceSecretKeys", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestSetDeviceSecretKeys), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommand>(queryResult.Data);
            else
                Debug.WriteLine($"SetDeviceSecretKeys: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommand>($"SetDeviceSecretKeys: {queryResult.StatusCode}");
        }

        public async Task<ResultCommand> SetDeviceType(RequestCommandSetDeviceType requestSetDeviceType)
        {
            RestRequest request = await PrepareQuery("SetDeviceType", Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(requestSetDeviceType), ParameterType.RequestBody);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommand>(queryResult.Data);
            else
                Debug.WriteLine($"SetDeviceType: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommand>($"SetDeviceType: {queryResult.StatusCode}");
        }

        public async Task<ResultCommandAvailableNetworks> GetAvailableNetworks()
        {
            RestRequest request = await PrepareQuery("GetAvailableNetworks", Method.GET);
            var queryResult = await _client.ExecuteTaskAsync<Command>(request);
            if (queryResult.IsSuccessful)
                return GetResultCommand<ResultCommandAvailableNetworks>(queryResult.Data);
            else
                Debug.WriteLine($"GetAvailableNetworks: {queryResult.StatusCode}");
            return ResultCommand.CreateFailedCommand<ResultCommandAvailableNetworks>($"GetAvailableNetworks: {queryResult.StatusCode}");
        }

        private T GetResultCommand<T>(Command resultCommand) where T : ResultCommand, new()
        {
            if(resultCommand.BaseCommand == CommandsEnum.ResultError)
                return new T
                {
                    IsSuccess = false,
                    ErrorMessage = "ResultError"
                };

            T result = JsonConvert.DeserializeObject<T>(resultCommand.Data.ToString()); //TODO broken after Data change from string to object 1/11/19
            if (!result.IsSuccess)
                return new T
                {
                    IsSuccess = false,
                    ErrorMessage = result.ErrorMessage
                };

            return result;
        }
    }
}
