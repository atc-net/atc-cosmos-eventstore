using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    internal interface IStreamWriter
    {
        Task<StreamResponse> WriteAsync(
            StreamId streamId,
            IReadOnlyCollection<object> events,
            StreamVersion version,
            StreamWriteOptions? options,
            CancellationToken cancellationToken);
    }
}