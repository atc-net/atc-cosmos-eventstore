namespace Atc.Cosmos.EventStore;

public interface IStreamMetadata
{
    StreamState State { get; }

    StreamId StreamId { get; }

    DateTimeOffset Timestamp { get; }

    StreamVersion Version { get; }

    string ETag { get; }
}