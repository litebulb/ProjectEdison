using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Edison.Core.Common.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChatUserRole
    {
        Consumer,
        Admin
    }
}
