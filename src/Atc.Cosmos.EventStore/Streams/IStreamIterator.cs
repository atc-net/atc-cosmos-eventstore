using System.Collections.Generic;
using System.Threading;

namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IStreamIterator
    {
        IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            StreamReadOptions? options,
            CancellationToken cancellationToken);
    }
}