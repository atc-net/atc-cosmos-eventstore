using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Converters
{
    public class EventNameConverter : JsonConverter<EventName>
    {
        public override EventName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value is null)
            {
                throw new JsonException();
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, EventName value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.Value);
    }
}