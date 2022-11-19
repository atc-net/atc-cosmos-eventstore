using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class EventDataConverterExecutor : IEventDataConverterPipelineExecutor
{
    private readonly IEventDataConverter converter;
    private readonly IEventDataConverterPipelineExecutor? next;

    public EventDataConverterExecutor(
        IEventDataConverter converter,
        IEventDataConverterPipelineExecutor? next)
    {
        this.converter = converter;
        this.next = next;
    }

    public object? Execute(
        IEventMetadata metadata,
        JsonElement json,
        JsonSerializerOptions options)
        => converter.Convert(
            metadata,
            json,
            options,
            () => next?.Execute(metadata, json, options));
}