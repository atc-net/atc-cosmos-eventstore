using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    internal interface IEventStoreInitializer
    {
        Task CreateEventStoreAsync(ThroughputProperties throughputProperties, CancellationToken cancellationToken);

        void CreateEventStore(ThroughputProperties throughputProperties);
    }
}