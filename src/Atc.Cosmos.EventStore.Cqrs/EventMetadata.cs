using System;

namespace Atc.Cosmos.EventStore.Cqrs
{
    public record EventMetadata(
        string EventId,
        EventStreamId StreamId,
        DateTimeOffset Timestamp,
        long Version,
        string? CorrelationId,
        string? CausationId);
}