using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Core.Common.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionStatus
    {
        Unknown = 0,
        NotStarted = 1,
        Error = 2,
        Skipped = 3,
        Success = 4
    }
}
