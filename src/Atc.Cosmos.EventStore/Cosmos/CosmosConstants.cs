namespace Atc.Cosmos.EventStore.Cosmos;

internal static class CosmosConstants
{
    /// <summary>
    /// Cosmos DB has a limit of 100 operations per batch and a limit of 2MB per batch.
    /// See https://docs.microsoft.com/en-us/azure/cosmos-db/concepts-limits#per-request-limits.
    /// </summary>
    internal const int BatchLimit = 100;

    /// <summary>
    /// A transaction will consist of at least 2 operations,
    /// one for the stream metadata and the other for the events.
    /// </summary>
    internal const int EventLimit = BatchLimit - 1;
}
