namespace GettingStarted.Projections;

public class ContainerInitializer : ICosmosContainerInitializer
{
    public const string Name = "read-models";

    public Task InitializeAsync(
        Database database,
        CancellationToken cancellationToken)
    {
        var options = new ContainerProperties
        {
            IndexingPolicy = new IndexingPolicy
            {
                Automatic = true,
                IndexingMode = IndexingMode.Consistent,
            },
            PartitionKeyPath = "/pk",
            Id = Name,
        };

        return database.CreateContainerIfNotExistsAsync(options, cancellationToken: cancellationToken);
    }
}