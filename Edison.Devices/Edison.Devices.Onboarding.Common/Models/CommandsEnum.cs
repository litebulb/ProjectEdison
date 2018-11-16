using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Devices.Onboarding.Common.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandsEnum
    {
        Unknown = 404,
        GetAvailableNetworks = 100,
        ConnectToNetwork = 101,
        DisconnectFromNetwork = 102,
        GetDeviceId = 103,
        GenerateCSR = 104,
        ProvisionDevice = 105,
        ListFirmwares = 106,
        SetDeviceType = 107,
        GetAccessPointSettings = 108,
        SetDeviceSecretKeys = 109,
        SetDeviceName = 110,

        ResultGetAvailableNetworks = 200,
        ResultConnectToNetwork = 201,
        ResultDisconnectFromNetwork = 202,
        ResultGetDeviceId = 203,
        ResultGenerateCSR = 204,
        ResultProvisionDevice = 205,
        ResultListFirmwares = 206,
        ResultSetDeviceType = 207,
        ResultGetAccessPointSettings = 208,
        ResultSetDeviceSecretKeys = 209,
        ResultSetDeviceName = 210,
        ResultError = 500
    }
}
