using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Cosmos;

internal record CheckpointDocument<TState>(
    [property: JsonPropertyName("id")] string Name,
    [property: JsonPropertyName("pk")] StreamId StreamId,
    StreamVersion StreamVersion,
    DateTimeOffset Timestamp,
    TState State);