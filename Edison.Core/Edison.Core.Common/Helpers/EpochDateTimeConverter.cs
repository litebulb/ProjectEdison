using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Edison.Core.Common
{
    public class EpochDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ulong seconds;
            if (value is DateTime dt)
            {
                if (!dt.Equals(DateTime.MinValue))
                    seconds = ToEpoch(dt);
                else
                    seconds = ulong.MinValue;
            }
            else
            {
                throw new Exception("Expected date object value.");
            }

            writer.WriteValue(seconds);
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Null)
                return null;

            if (reader.TokenType != JsonToken.Integer)
            {
                throw new Exception(
                    String.Format("Unexpected token parsing date. Expected Integer, got {0}.",
                    reader.TokenType));
            }

            long seconds = (long)reader.Value;
            //if (seconds == 0)
            //    return DateTime.MinValue;
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }

        public ulong ToEpoch(DateTime date)
        {
            if (date == null) return ulong.MinValue;
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan epochTimeSpan = date - epoch;
            return (ulong)epochTimeSpan.TotalSeconds;
        }
    }
}
