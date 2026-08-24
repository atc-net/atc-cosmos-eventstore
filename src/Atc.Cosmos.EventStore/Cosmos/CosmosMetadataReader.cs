namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosMetadataReader : IStreamMetadataReader
{
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly IDateTimeProvider timeProvider;
    private readonly CosmosEventSerializer serializer;

    public CosmosMetadataReader(
        IEventStoreContainerProvider containerProvider,
        IDateTimeProvider timeProvider,
        CosmosEventSerializer serializer)
    {
        this.containerProvider = containerProvider;
        this.timeProvider = timeProvider;
        this.serializer = serializer;
    }

    public async Task<IStreamMetadata> GetAsync(
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        var container = containerProvider.GetStreamContainer();

        // Read as a stream so a stream that does not exist yet is just a 404 response
        // instead of a thrown exception. Throwing on the not-found path is expensive
        // and fills the debug output with exceptions that were never actually a
        // problem - and here it happens every single time a new stream is created.
        using var response = await container
            .ReadItemStreamAsync(
                StreamMetadata.StreamMetadataId,
                new PartitionKey(streamId.Value),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new StreamMetadata(
                StreamMetadata.StreamMetadataId,
                streamId.Value,
                streamId,
                StreamVersion.StartOfStream,
                StreamState.New,
                timeProvider.GetDateTime());
        }

        response.EnsureSuccessStatusCode();

        var metadata = serializer
            .FromStream<StreamMetadata>(response.Content)
            ?? throw new InvalidOperationException(
                $"Unable to deserialize stream metadata for stream '{streamId.Value}'.");

        metadata.ETag = response.Headers.ETag;

        return metadata;
    }
}