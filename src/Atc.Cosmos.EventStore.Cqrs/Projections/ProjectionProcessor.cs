using System.Diagnostics.CodeAnalysis;
using Atc.Cosmos.EventStore.Cqrs.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal class ProjectionProcessor<TProjection> : IProjectionProcessor<TProjection>
    where TProjection : IProjection
{
    private readonly IReadOnlyCollection<ProjectionFilter> filters;
    private readonly IProjectionFactory projectionFactory;
    private readonly IProjectionTelemetry telemetry;
    private readonly IProjectionMetadata projectionMetadata;
    private readonly string projectionName;

    public ProjectionProcessor(
        IProjectionOptionsFactory optionsFactory,
        IProjectionFactory projectionFactory,
        IProjectionTelemetry telemetry)
    {
        this.projectionFactory = projectionFactory;
        this.telemetry = telemetry;
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
        var groupedEvents = batch
            .Where(e => filters.Any(f => f.Evaluate(e.Metadata.StreamId)))
            .GroupBy(e => e.Metadata.StreamId)
            .ToArray();
        if (groupedEvents.Length == 0)
        {
            telemetry.ProjectionSkipped(projectionName);

            return ProjectionAction.Continue;
        }

        using var batchTelemetry = telemetry.StartBatch(projectionName, groupedEvents.Length);

        var projection = projectionFactory
            .GetProjection<TProjection>();

        foreach (var events in groupedEvents)
        {
            using var operation = batchTelemetry.StartProjection(events.Key);

            if (!projectionMetadata.CanConsumeOneOrMoreEvents(events))
            {
                // Skip if projection is not consuming any of the events.
                operation.ProjectionSkipped();
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

                operation.ProjectionCompleted();
            }
            catch (Exception ex)
            {
                operation.ProjectionFailed(ex);

                return await projection
                    .FailedAsync(ex, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        return ProjectionAction.Continue;
    }
}