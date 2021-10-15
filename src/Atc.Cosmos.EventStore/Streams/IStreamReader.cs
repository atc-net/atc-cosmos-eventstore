using System.Collections.Generic;
using System.Threading;

namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IStreamReader
    {
        IAsyncEnumerable<IEvent> ReadAsync(
            StreamId streamId,
            StreamVersion fromVersion,
            StreamReadFilter? filter,
            CancellationToken cancellationToken);
    }
}