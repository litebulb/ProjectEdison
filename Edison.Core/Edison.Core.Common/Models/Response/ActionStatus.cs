using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Core.Common.Models
{
    [Flags]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionStatus
    {
        Unknown = 0,
        NotStarted = 1,
        Error = 2,
        Skipped = 4,
        Success = 8
    }
}
