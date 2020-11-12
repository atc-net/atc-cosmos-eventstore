using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BigBang.Cosmos.EventStore.Serialization
{
    /// <summary>
    /// Responsible for converting an event envelope to and from json without loosing underlying event type.
    /// </summary>
    public class EventConverter : JsonConverter<Event>
    {
        private readonly IEventTypeNameMapper mapper;

        public EventConverter(IEventTypeNameMapper mapper)
        {
            this.mapper = mapper;
        }

        public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);

            if (!jsonDocument.RootElement.TryGetProperty(EventPropertyNames.Properties, out var properties))
            {
                throw new JsonException();
            }

            if (!properties.TryGetProperty(EventPropertyNames.EventName, out var name))
            {
                throw new JsonException();
            }

            var type = mapper.GetEventType(name.GetString() ?? string.Empty);

            var jsonObject = jsonDocument.RootElement.GetRawText();
            var result = (Event)JsonSerializer.Deserialize(jsonObject, type, options)!;

            return result;
        }

        public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
        {
            if (!(value is Event<object> evt))
            {
                throw new ArgumentException("Value to write must be of type Event<object>,");
            }

            evt.Properties.Name = mapper.GetEventName(evt.Data.GetType());
            JsonSerializer.Serialize(writer, evt, options);
        }
    }
}