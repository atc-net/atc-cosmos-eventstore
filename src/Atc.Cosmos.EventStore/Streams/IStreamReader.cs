using System.Collections.Generic;
using System.Threading;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamReader
    {
        IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            CancellationToken cancellationToken);
    }
}