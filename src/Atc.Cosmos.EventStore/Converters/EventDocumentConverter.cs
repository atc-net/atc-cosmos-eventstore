using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.Converters
{
    /// <summary>
    /// Responsible for converting an event envelope to and from json without loosing underlying event type.
    /// </summary>
    internal class EventDocumentConverter : JsonConverter<EventDocument>
    {
        private readonly IEventTypeProvider typeProvider;

        public EventDocumentConverter(IEventTypeProvider typeProvider)
        {
            this.typeProvider = typeProvider;
        }

        public override EventDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            using var jsonDocument = JsonDocument.ParseValue(ref reader);

            if (!jsonDocument.RootElement.TryGetProperty(EventMetadataNames.Properties, out var properties))
            {
                // If we are reading the meta-data document, then skip it.
                if (jsonDocument.RootElement.TryGetProperty(EventMetadataNames.Id, out var id)
                    && id.GetString() == StreamMetadata.StreamMetadataId)
                {
                    return default!;
                }

                throw new JsonException();
            }

            if (!properties.TryGetProperty(EventMetadataNames.EventName, out var name))
            {
                throw new JsonException();
            }

            var result = (EventDocument)JsonSerializer
                .Deserialize(
                    jsonDocument.RootElement.GetRawText(),
                    MakeGenericEventDocumentType(
                        typeProvider.GetEventType(
                            name.GetString() ?? string.Empty)),
                    options)!;

            return result;
        }

        public override void Write(Utf8JsonWriter writer, EventDocument value, JsonSerializerOptions options)
        {
            if (value is not EventDocument<object> evt)
            {
                throw new ArgumentException("Value to write must be of type Event<object>.", nameof(value));
            }

            JsonSerializer.Serialize(writer, evt, options);
        }

        private static Type MakeGenericEventDocumentType(Type type)
            => typeof(EventDocument<>).MakeGenericType(type);
    }
}