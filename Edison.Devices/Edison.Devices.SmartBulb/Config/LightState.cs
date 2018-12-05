using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Devices.SmartBulb
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LightState
    {
        Continuous = 0,
        Flashing = 1
    }
}
