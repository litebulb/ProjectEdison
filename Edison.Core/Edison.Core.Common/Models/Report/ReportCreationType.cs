using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Core.Common.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportCreationType
    {
        All = 0,
        Events = 1,
        Responses = 2
    }
}
