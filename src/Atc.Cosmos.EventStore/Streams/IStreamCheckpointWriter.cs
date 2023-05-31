namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamCheckpointWriter
{
    Task WriteAsync(
        string name,
        StreamId streamId,
        StreamVersion streamVersion,
        object? state,
        CancellationToken cancellationToken);
}