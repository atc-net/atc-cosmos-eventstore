using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Converters
{
    internal class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var timespan = reader.GetString();
            return TimeSpan.TryParse(timespan, CultureInfo.InvariantCulture, out var ts)
                ? ts
                : throw new JsonException("Value is not a valid timespan format");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}