using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Mobile.Admin.Client.Core.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CommandsEnum
    {
        Unknown = 404,
        GetAvailableNetworks = 100,
        ConnectToNetwork = 101,
        DisconnectFromNetwork = 102,
        GetDeviceId = 103,
        GetGeneratedCSR = 104,
        ProvisionDevice = 105,
        GetFirmwares = 106,
        SetDeviceType = 107,
        GetAccessPointSettings = 108,
        SetDeviceSecretKeys = 109,
        SetDeviceName = 110,
        DisableAccessPointTimeout = 111,
        EnableAccessPointTimeout = 112,
        EnableEncryption = 113,
        DisableEncryption = 114,
        GetEncryptionState = 115,

        ResultGetAvailableNetworks = 200,
        ResultConnectToNetwork = 201,
        ResultDisconnectFromNetwork = 202,
        ResultGetDeviceId = 203,
        ResultGetGeneratedCSR = 204,
        ResultProvisionDevice = 205,
        ResultGetFirmwares = 206,
        ResultSetDeviceType = 207,
        ResultGetAccessPointSettings = 208,
        ResultSetDeviceSecretKeys = 209,
        ResultSetDeviceName = 210,
        ResultDisableAccessPointTimeout = 211,
        ResultEnableAccessPointTimeout = 212,
        ResultEnableEncryption = 213,
        ResultDisableEncryption = 214,
        ResultGetEncryptionState = 215,
        ResultError = 500
    }
}
