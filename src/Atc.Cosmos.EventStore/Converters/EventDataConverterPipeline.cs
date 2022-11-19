using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal class EventDataConverterPipeline
{
    private readonly IEventDataConverterPipelineExecutor? pipeline;

    public EventDataConverterPipeline(
        IEventDataConverterPipelineExecutor? pipeline)
        => this.pipeline = pipeline;

    public object Convert(
        IEventMetadata metadata,
        JsonElement data,
        JsonSerializerOptions options)
        => pipeline?.Execute(metadata, data, options)
        ?? new UnknownEvent(
            data.GetRawText());
}
