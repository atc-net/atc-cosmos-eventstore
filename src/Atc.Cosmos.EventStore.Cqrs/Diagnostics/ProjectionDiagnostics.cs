using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

[SuppressMessage(
    "Performance",
    "CA1812:Avoid uninstantiated internal classes",
    Justification = "Class is used, rule is broken")]
internal class ProjectionDiagnostics : IProjectionDiagnostics
{
    public void ProcessingGroupedEvents(
        string projectionName,
        IGrouping<StreamId, IEvent>[] groupedEvents,
        IEnumerable<IEvent> batch)
    {
        // Ignore
    }

    public IProjectionProcessOperation StartStreamProjection(
        string projectionName,
        StreamId key)
        => new ProjectionProcessOperation();
}