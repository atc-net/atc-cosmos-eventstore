using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Cosmos;

internal record CheckpointDocument(
    [property: JsonPropertyName("id")] string Name,
    [property: JsonPropertyName("pk")] StreamId StreamId,
    StreamVersion StreamVersion,
    DateTimeOffset Timestamp,
    object? State);