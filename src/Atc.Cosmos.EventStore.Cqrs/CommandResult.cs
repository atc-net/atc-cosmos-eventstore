namespace Atc.Cosmos.EventStore.Cqrs;

public record CommandResult(
    EventStreamId Id,
    StreamVersion Version,
    ResultType Result,
    object? Response = default);