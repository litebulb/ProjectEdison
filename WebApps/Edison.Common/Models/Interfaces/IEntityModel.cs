using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Edison.Common.Models
{
    public interface IEntityModel
    {
        [JsonProperty(PropertyName = "id")]
        string Id { get; set; }
        DateTime Date { get; set; }
        string ETag { get; set; }
    }
}
