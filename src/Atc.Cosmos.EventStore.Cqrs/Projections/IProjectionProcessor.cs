using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public interface IProjectionProcessor<TProjection>
        where TProjection : IProjection
    {
        ValueTask ProcessBatchAsync(
            IEnumerable<IEvent> batch,
            CancellationToken cancellationToken);
    }
}