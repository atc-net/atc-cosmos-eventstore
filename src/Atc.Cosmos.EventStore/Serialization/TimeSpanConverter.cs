using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BigBang.Cosmos.EventStore.Serialization
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timespan = reader.GetString();
            return TimeSpan.TryParse(timespan, out var ts)
                ? ts
                : throw new JsonException("Value is not a valid timespan format");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}