namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamBatchWriter
{
    Task<IStreamMetadata> WriteAsync(
        StreamBatch batch,
        CancellationToken cancellationToken);
}