namespace Atc.Cosmos.EventStore.Cosmos;

public static class CosmosConstants
{
    /// <summary>
    /// Cosmos DB has a limit of 100 operations per batch and a limit of 2MB per batch.
    /// See https://docs.microsoft.com/en-us/azure/cosmos-db/concepts-limits#per-request-limits.
    /// </summary>
    public const int BatchLimit = 100;

    /// <summary>
    /// A transaction will consist of at least 2 operations,
    /// one for the stream metadata and the other for the events.
    /// Since there is no efficient way to determine the size of the total payload, 
    /// </summary>
    public const int EventLimit = BatchLimit / 2;
}
