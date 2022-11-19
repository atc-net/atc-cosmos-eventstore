using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos;

public interface IEventStoreInitializer
{
    Task CreateEventStoreAsync(
        ThroughputProperties throughputProperties,
        CancellationToken cancellationToken);

    void CreateEventStore(
        ThroughputProperties throughputProperties);
}