using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    internal interface IProjectionProcessor<TProjection>
        where TProjection : IProjection
    {
        Task ProcessBatchAsync(
            IEnumerable<IEvent> batch,
            CancellationToken cancellationToken);
    }
}