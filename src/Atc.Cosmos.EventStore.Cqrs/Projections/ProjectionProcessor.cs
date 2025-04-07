using System.Diagnostics.CodeAnalysis;
using Atc.Cosmos.EventStore.Cqrs.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal class ProjectionProcessor<TProjection> : IProjectionProcessor<TProjection>
    where TProjection : IProjection
{
    private readonly IReadOnlyCollection<ProjectionFilter> filters;
    private readonly IProjectionTelemetry telemetry;
    private readonly ProjectionMetadata<TProjection> projectionMetadata;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly string projectionName;

    public ProjectionProcessor(
        IProjectionOptionsFactory optionsFactory,
        IProjectionTelemetry telemetry,
        ProjectionMetadata<TProjection> projectionMetadata,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.telemetry = telemetry;
        this.projectionMetadata = projectionMetadata;
        this.serviceScopeFactory = serviceScopeFactory;
        filters = optionsFactory
            .GetOptions<TProjection>()
            .Filters;
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

        foreach (var events in groupedEvents)
        {
            using var operation = batchTelemetry.StartProjection(events.Key);

            if (!projectionMetadata.CanConsumeOneOrMoreEvents(events))
            {
                // Skip if projection is not consuming any of the events.
                operation.ProjectionSkipped();
                continue;
            }

            var eventStreamId = EventStreamId.FromStreamId(events.Key);
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var projectionFactory = scope.ServiceProvider.GetRequiredService<IProjectionFactory>();
            var projection = await projectionFactory.CreateAsync<TProjection>(eventStreamId, cancellationToken);

            try
            {
                await projection
                    .InitializeAsync(
                        eventStreamId,
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