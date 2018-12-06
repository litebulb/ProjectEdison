using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Core.Common.Models
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ReportCreationType
    {
        Events = 1,
        Responses = 2,
        Conversations = 4
    }
}
