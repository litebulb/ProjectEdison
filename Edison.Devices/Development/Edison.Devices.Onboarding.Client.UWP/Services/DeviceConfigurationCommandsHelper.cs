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
    public class DeviceConfigurationCommandsHelper : IDeviceConfigurationCommandsHelper
    {
        private readonly IAccessPointHelper _accessPoint;

        public DeviceConfigurationCommandsHelper(IAccessPointHelper accessPointHelper)
        {
            _accessPoint = accessPointHelper;
        }

        public async Task<IEnumerable<string>> ListFirmwares()
        {
            if (!_accessPoint.IsConnected) { return new List<string>(); }

            var listFirmwaresRequest = new Command() { BaseCommand = CommandsEnum.ListFirmwares };
            await _accessPoint.SendRequest(listFirmwaresRequest);
            var listFirmwaresResponse = await _accessPoint.GetNextRequest();
            if (listFirmwaresResponse != null && listFirmwaresResponse.BaseCommand == CommandsEnum.ResultListFirmwares)
            {
                ResultCommandListFirmwares dataListFirmwares = JsonConvert.DeserializeObject<ResultCommandListFirmwares>(listFirmwaresResponse.Data);
                if (dataListFirmwares != null)
                    return dataListFirmwares.Firmwares;
            }

            return new List<string>();
        }

        public async Task<bool> SetDeviceType(string deviceType)
        {
            if (!_accessPoint.IsConnected) { return false; }

            var setDeviceTypeRequest = new Command() { BaseCommand = CommandsEnum.SetDeviceType, Data = deviceType };
            await _accessPoint.SendRequest(setDeviceTypeRequest);
            var setDeviceTypeResponse = await _accessPoint.GetNextRequest();
            if (setDeviceTypeResponse != null && setDeviceTypeResponse.BaseCommand == CommandsEnum.ResultSetDeviceType)
            {
                ResultCommand dataSetDeviceType = JsonConvert.DeserializeObject<ResultCommand>(setDeviceTypeResponse.Data);
                if (dataSetDeviceType != null)
                    return dataSetDeviceType.IsSuccess;
            }

            return false;
        }
    }
}
