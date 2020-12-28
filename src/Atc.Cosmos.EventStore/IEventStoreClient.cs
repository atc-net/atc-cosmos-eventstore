using System.Threading;
using System.Threading.Tasks;

namespace BigBang.Cosmos.EventStore
{
    public interface IEventStoreClient
    {
        IEventStream GetVersionedStream(string streamName);

        IEventStream GetTimeseriesStream(string streamName);

        Task InitializeStoreAsync(CancellationToken cancellationToken = default);
    }
}