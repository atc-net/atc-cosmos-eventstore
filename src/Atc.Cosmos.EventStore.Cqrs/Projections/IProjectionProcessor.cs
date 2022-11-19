using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore.Cqrs.Projections;

[SuppressMessage(
    "Major Code Smell",
    "S2326:Unused type parameters should be removed",
    Justification = "Interface is used with DI to distinguish one projection processor from another")]
internal interface IProjectionProcessor<TProjection>
    where TProjection : IProjection
{
    Task<ProjectionAction> ProcessBatchAsync(
        IEnumerable<IEvent> batch,
        CancellationToken cancellationToken);
}