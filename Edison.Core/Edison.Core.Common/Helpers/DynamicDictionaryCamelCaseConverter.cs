using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Edison.Core.Common
{
    public class DynamicDictionaryCamelCaseConverter : JsonConverter
    {
        public override bool CanRead => false;

        private static readonly JsonSerializer _Serializer = new JsonSerializer
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject.FromObject(value, _Serializer).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(Dictionary<string,object>))
                return true;
            return false;
        }
    }
}
