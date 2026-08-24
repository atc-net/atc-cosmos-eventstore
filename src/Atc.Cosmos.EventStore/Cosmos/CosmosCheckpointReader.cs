namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosCheckpointReader : IStreamCheckpointReader
{
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly CosmosEventSerializer serializer;

    public CosmosCheckpointReader(
        IEventStoreContainerProvider containerProvider,
        CosmosEventSerializer serializer)
    {
        this.containerProvider = containerProvider;
        this.serializer = serializer;
    }

    public async Task<Checkpoint<TState>?> ReadAsync<TState>(
        string name,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        var container = containerProvider.GetIndexContainer();

        // Read as a stream so a missing checkpoint is just a 404 response instead of a
        // thrown exception. Throwing on the not-found path is expensive and fills the
        // debug output with exceptions that were never actually a problem.
        using var response = await container
            .ReadItemStreamAsync(
                name,
                new PartitionKey(streamId.Value),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var document = serializer
            .FromStream<CheckpointDocument<TState>>(response.Content);

        if (document is null)
        {
            return null;
        }

        return new Checkpoint<TState>(
            document.Name,
            document.StreamId,
            document.StreamVersion,
            document.Timestamp,
            document.State);
    }
}