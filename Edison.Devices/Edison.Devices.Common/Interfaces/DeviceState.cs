using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Devices.Common
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DeviceState
    {
        Off = 0,
        On = 1
    }
}
