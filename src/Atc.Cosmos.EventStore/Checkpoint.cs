namespace Atc.Cosmos.EventStore;

/// <summary>
/// Represents a named checkpoint within a stream.
/// </summary>
/// <param name="Name">Name of checkpoint.</param>
/// <param name="StreamId">Id of stream.</param>
/// <param name="StreamVersion">Version within the stream.</param>
/// <param name="Timestamp">When the checkpoint was created.</param>
public record Checkpoint(
    string Name,
    StreamId StreamId,
    StreamVersion StreamVersion,
    DateTimeOffset Timestamp);