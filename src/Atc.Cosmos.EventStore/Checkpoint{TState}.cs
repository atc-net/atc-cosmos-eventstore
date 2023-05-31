namespace Atc.Cosmos.EventStore;

/// <summary>
/// Represents a named checkpoint and state within a stream.
/// </summary>
/// <typeparam name="TState">Type of state.</typeparam>
/// <param name="Name">Name of checkpoint.</param>
/// <param name="StreamId">Id of stream.</param>
/// <param name="StreamVersion">Version within the stream.</param>
/// <param name="Timestamp">When the checkpoint was created.</param>
/// <param name="State">Checkpoint state to set.</param>
public record Checkpoint<TState>(
    string Name,
    StreamId StreamId,
    StreamVersion StreamVersion,
    DateTimeOffset Timestamp,
    TState State)
    : Checkpoint(Name, StreamId, StreamVersion, Timestamp);