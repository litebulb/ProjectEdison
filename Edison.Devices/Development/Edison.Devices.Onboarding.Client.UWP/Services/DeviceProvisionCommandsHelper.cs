using Edison.Devices.Onboarding.Client.Interfaces;
using Edison.Devices.Onboarding.Client.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Edison.Devices.Onboarding.Client.Services;
using Edison.Devices.Onboarding.Common.Models;
using Newtonsoft.Json;
using Edison.Devices.Onboarding.Common.Models.CommandModels;

namespace Edison.Devices.Onboarding.Client.UWP
{
    public class DeviceProvisionCommandsHelper : IDeviceProvisionCommandsHelper
    {
        private readonly IAccessPointHelper _accessPoint;

        public DeviceProvisionCommandsHelper(IAccessPointHelper accessPointHelper)
        {
            _accessPoint = accessPointHelper;
        }

        /// <summary>
        /// Retrieve DeviceId of the Device
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetDeviceId()
        {
            if (!_accessPoint.IsConnected) { return string.Empty; }

            var networkRequest = new Command() { BaseCommand = CommandsEnum.GetDeviceId };
            await _accessPoint.SendRequest(networkRequest);              
            var getDeviceIdResponse = await _accessPoint.GetNextRequest();
            if (getDeviceIdResponse != null && getDeviceIdResponse.BaseCommand == CommandsEnum.ResultGetDeviceId)
            {
                ResultCommandGetDeviceId dataGetDeviceId = JsonConvert.DeserializeObject<ResultCommandGetDeviceId>(getDeviceIdResponse.Data);
                if(dataGetDeviceId != null)
                    return dataGetDeviceId.DeviceId;
            }

            return string.Empty;
        }

        /// <summary>
        /// Command Provision Device
        /// The Certificate Model must be retrieve from /Certificates/DeviceProvisioning/ endpoint
        /// </summary>
        /// <param name="deviceCertificateModel">DeviceCertificateModel certificate</param>
        /// <returns>True if autoprovisioning succeeded</returns>
        public async Task<bool> ProvisionDevice(DeviceCertificateModel deviceCertificateModel)
        {
            if (!_accessPoint.IsConnected) { return false; }

            var networkRequest = new Command() { BaseCommand = CommandsEnum.ProvisionDevice, Data = JsonConvert.SerializeObject(deviceCertificateModel) };
            await _accessPoint.SendRequest(networkRequest);            
            var provisionDeviceResponse = await _accessPoint.GetNextRequest();
            if (provisionDeviceResponse != null && provisionDeviceResponse.BaseCommand == CommandsEnum.ResultProvisionDevice)
            {
                ResultCommand dataProvisionDevice = JsonConvert.DeserializeObject<ResultCommand>(provisionDeviceResponse.Data);
                if (dataProvisionDevice != null)
                {
                    if (!dataProvisionDevice.IsSuccess)
                        Debug.WriteLine($"ProvisionDevice: {dataProvisionDevice.ErrorMessage}");
                    return dataProvisionDevice.IsSuccess;
                }
                return true;
            }

            return true;
        }

        public async Task<string> GenerateCSR()
        {
            if (!_accessPoint.IsConnected) { return string.Empty; }

            var networkRequest = new Command() { BaseCommand = CommandsEnum.GenerateCSR };
            await _accessPoint.SendRequest(networkRequest);              
            var generateCSRResponse = await _accessPoint.GetNextRequest();
            if (generateCSRResponse != null && generateCSRResponse.BaseCommand == CommandsEnum.ResultGenerateCSR)
            {
                ResultCommandGenerateCSR dataGenerateCSR = JsonConvert.DeserializeObject<ResultCommandGenerateCSR>(generateCSRResponse.Data);
                if (dataGenerateCSR != null)
                    return dataGenerateCSR.Csr;
            }

            return string.Empty;
        }
    }
}
