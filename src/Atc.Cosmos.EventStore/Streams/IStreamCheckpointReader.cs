namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamCheckpointReader
{
    Task<Checkpoint<TState>?> ReadAsync<TState>(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken);
}