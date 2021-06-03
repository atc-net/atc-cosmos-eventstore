using System.Collections.Generic;
using System.Threading;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamIterator
    {
        IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            CancellationToken cancellationToken);
    }
}