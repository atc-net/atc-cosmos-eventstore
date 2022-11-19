using System.Text.Json;

namespace Atc.Cosmos.EventStore.Converters;

internal interface IEventDataConverterPipelineExecutor
{
    object? Execute(
        IEventMetadata metadata,
        JsonElement json,
        JsonSerializerOptions options);
}
