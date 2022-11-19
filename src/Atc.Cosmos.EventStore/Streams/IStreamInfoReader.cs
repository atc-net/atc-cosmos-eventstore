namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamInfoReader
{
    Task<IStreamMetadata> ReadAsync(
        StreamId streamId,
        CancellationToken cancellationToken = default);
}