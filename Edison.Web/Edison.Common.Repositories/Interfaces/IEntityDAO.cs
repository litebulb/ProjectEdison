using Edison.Core.Common;
using Newtonsoft.Json;
using System;

namespace Edison.Common.Interfaces
{
    public interface IEntityDAO
    {
        [JsonProperty(PropertyName = "id")]
        string Id { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        DateTime CreationDate { get; set; }

        [JsonConverter(typeof(EpochDateTimeConverter))]
        DateTime UpdateDate { get; set; }

        [JsonProperty(PropertyName = "_etag")]
        string ETag { get; set; }
    }
}
