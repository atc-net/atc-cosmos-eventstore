using System.Text.Json;
using System.Text.Json.Serialization;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.Converters;

/// <summary>
/// Responsible for converting an event envelope to and from json without loosing underlying event type.
/// </summary>
internal class EventDocumentConverter : JsonConverter<EventDocument>
{
    private readonly EventDataConverterPipeline pipeline;

    public EventDocumentConverter(
        EventDataConverterPipeline pipeline)
        => this.pipeline = pipeline;

    public override EventDocument Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        // If we are reading the meta-data document, then skip it.
        if (jsonDocument.RootElement.TryGetProperty(EventMetadataNames.Id, out var id)
            && id.GetString() == StreamMetadata.StreamMetadataId)
        {
            return default!;
        }

        if (jsonDocument.RootElement.TryGetProperty(EventMetadataNames.Properties, out var properties))
        {
            var metadata = properties.Deserialize<EventMetadata>(options);
            if (metadata is not null && jsonDocument.RootElement.TryGetProperty(EventMetadataNames.Data, out var data))
            {
                var typeData = pipeline
                    .Convert(metadata, data, options);

                return new EventDocument<object>(typeData, metadata);
            }
        }

        return new EventDocument<object>(
            new FaultedEvent(
                jsonDocument.RootElement.GetRawText(),
                null),
            new EventMetadata());
    }

    public override void Write(
        Utf8JsonWriter writer,
        EventDocument value,
        JsonSerializerOptions options)
    {
        if (value is not EventDocument<object> evt)
        {
            throw new ArgumentException("Value to write must be of type Event<object>.", nameof(value));
        }

        JsonSerializer.Serialize(writer, evt, options);
    }
}