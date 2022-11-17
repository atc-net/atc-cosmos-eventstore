using System;
using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class EventDataConverterPipeline
{
    private readonly Func<IEventMetadata, JsonElement, JsonSerializerOptions, object?> pipeline;

    public EventDataConverterPipeline(
        Func<IEventMetadata, JsonElement, JsonSerializerOptions, object?> pipeline)
        => this.pipeline = pipeline;

    public object Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options)
        => pipeline.Invoke(metadata, data, options)
        ?? new UnknownEvent(
            data.GetRawText());
}
