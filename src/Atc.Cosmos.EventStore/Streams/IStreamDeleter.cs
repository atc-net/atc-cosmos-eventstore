namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamDeleter
{
    Task DeleteAsync(
        StreamId streamId,
        CancellationToken cancellationToken);
}