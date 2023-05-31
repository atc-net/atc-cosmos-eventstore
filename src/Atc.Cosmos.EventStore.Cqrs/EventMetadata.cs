namespace Atc.Cosmos.EventStore.Cqrs;

public record EventMetadata(
    EventStreamId StreamId,
    DateTimeOffset Timestamp,
    long Version,
    string? CorrelationId,
    string? CausationId);