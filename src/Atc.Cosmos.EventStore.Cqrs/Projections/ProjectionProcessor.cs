using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public class ProjectionProcessor<TProjection> : IProjectionProcessor<TProjection>
        where TProjection : IProjection
    {
        private readonly IReadOnlyCollection<StreamFilter> filters;
        private readonly IProjectionFactory projectionFactory;
        private readonly IProjectionMetadata projectionMetadata;

        public ProjectionProcessor(
            IProjectionOptionsFactory optionsFactory,
            IProjectionFactory projectionFactory)
        {
            this.projectionFactory = projectionFactory;
            filters = optionsFactory
                .GetOptions<TProjection>()
                .Filters;
            projectionMetadata = projectionFactory
                .GetProjectionMetadata<TProjection>();
        }

        public async ValueTask ProcessBatchAsync(
            IEnumerable<IEvent> batch,
            CancellationToken cancellationToken)
        {
            var groupedEvents = batch
                .Where(e => filters.Any(f => f.Evaluate(e.Properties.StreamId)))
                .GroupBy(e => e.Properties.StreamId)
                .ToArray();

            foreach (var events in groupedEvents)
            {
                if (!projectionMetadata.CanConsumeOneOrMoreEvents(events))
                {
                    // Skip if projection is not consuming any of the events.
                    continue;
                }

                var projection = projectionFactory
                    .GetProjection<TProjection>();

                await projection
                    .InitializeAsync(
                        events.Key,
                        cancellationToken)
                    .ConfigureAwait(false);

                await projectionMetadata
                    .ConsumeEventsAsync(events, projection, cancellationToken)
                    .ConfigureAwait(false);

                await projection
                    .CompleteAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}