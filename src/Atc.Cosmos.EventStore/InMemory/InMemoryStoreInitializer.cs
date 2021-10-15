using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cosmos;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.InMemory
{
    internal class InMemoryStoreInitializer : IEventStoreInitializer
    {
        public void CreateEventStore(ThroughputProperties throughputProperties)
        {
            // No initialization required for in-memory store.
        }

        public Task CreateEventStoreAsync(ThroughputProperties throughputProperties, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}