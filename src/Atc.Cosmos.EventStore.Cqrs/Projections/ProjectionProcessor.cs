using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cqrs.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    internal class ProjectionProcessor<TProjection> : IProjectionProcessor<TProjection>
        where TProjection : IProjection
    {
        private readonly IReadOnlyCollection<ProjectionFilter> filters;
        private readonly IProjectionFactory projectionFactory;
        private readonly IProjectionDiagnostics diagnostics;
        private readonly IProjectionMetadata projectionMetadata;
        private readonly string projectionName;

        public ProjectionProcessor(
            IProjectionOptionsFactory optionsFactory,
            IProjectionFactory projectionFactory,
            IProjectionDiagnostics diagnostics)
        {
            this.projectionFactory = projectionFactory;
            this.diagnostics = diagnostics;
            filters = optionsFactory
                .GetOptions<TProjection>()
                .Filters;
            projectionMetadata = projectionFactory
                .GetProjectionMetadata<TProjection>();
            projectionName = typeof(TProjection).Name;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By Design")]
        public async Task<ProjectionAction> ProcessBatchAsync(
            IEnumerable<IEvent> batch,
            CancellationToken cancellationToken)
        {
            var projection = projectionFactory
                .GetProjection<TProjection>();

            try
            {
                var groupedEvents = batch
                    .Where(e => filters.Any(f => f.Evaluate(e.Metadata.StreamId)))
                    .GroupBy(e => e.Metadata.StreamId)
                    .ToArray();

                diagnostics.ProcessingGroupedEvents(projectionName, groupedEvents, batch);

                foreach (var events in groupedEvents)
                {
                    var operation = diagnostics.StartStreamProjection(projectionName, events.Key);
                    if (!projectionMetadata.CanConsumeOneOrMoreEvents(events))
                    {
                        // Skip if projection is not consuming any of the events.
                        operation.ProjectionSkipped(events);
                        continue;
                    }

                    try
                    {
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

                        operation.ProjectionCompleted(events);
                    }
                    catch (Exception ex)
                    {
                        operation.ProjectionFailed(events, ex);

                        return await projection
                            .FailedAsync(ex, cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                return await projection
                    .FailedAsync(ex, cancellationToken)
                    .ConfigureAwait(false);
            }

            return ProjectionAction.Continue;
        }
    }
}