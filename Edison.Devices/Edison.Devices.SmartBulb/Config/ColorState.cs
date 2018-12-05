using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Devices.SmartBulb
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ColorState
    {
        Unknown = 0,
        Off = 1,
        White = 2,
        Red = 3,
        Green = 4,
        Blue = 5,
        Yellow = 6,
        Purple = 7,
        Cyan = 8
    }
}
