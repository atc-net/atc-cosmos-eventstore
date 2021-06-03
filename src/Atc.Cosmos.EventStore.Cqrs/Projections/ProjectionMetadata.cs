using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cqrs.Internal;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public class ProjectionMetadata<TProjection> :
        ConsumeEventMetadata,
        IProjectionMetadata
        where TProjection : IProjection
    {
        public ProjectionMetadata()
            : base(typeof(TProjection))
        {
        }

        public bool CanConsumeOneOrMoreEvents(IEnumerable<IEvent> events)
            => events.Any(evt => CanConsumeEvent(evt));

        public async ValueTask ConsumeEventsAsync(
            IEnumerable<IEvent> events,
            IProjection projection,
            CancellationToken cancellationToken)
        {
            foreach (var evt in events)
            {
                await ConsumeAsync(
                        evt,
                        projection,
                        cancellationToken)
                   .ConfigureAwait(false);
            }
        }
    }
}