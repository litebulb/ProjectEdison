using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Devices.Onboarding.Common.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandsEnum
    {
        Unknown = 0,
        GetAvailableNetworks = 1,
        ConnectToNetwork = 3,
        DisconnectFromNetwork = 5,
        GetDeviceId = 7,
        GenerateCSR = 9,
        ProvisionDevice = 11,
        ListFirmwares = 13,
        SetAccessPoint = 15,
        SetDevicePassword = 17,
        SetDeviceType = 19,
        GetAccessPointSettings = 21,

        ResultGetAvailableNetworks = 2,
        ResultConnectToNetwork = 4,
        ResultDisconnectFromNetwork = 6,
        ResultGetDeviceId = 8,
        ResultGenerateCSR = 10,
        ResultProvisionDevice = 12,
        ResultListFirmwares = 14,
        ResultSetAccessPoint = 16,
        ResultSetDevicePassword = 18,
        ResultSetDeviceType = 20,
        ResultGetAccessPointSettings = 22
    }
}
