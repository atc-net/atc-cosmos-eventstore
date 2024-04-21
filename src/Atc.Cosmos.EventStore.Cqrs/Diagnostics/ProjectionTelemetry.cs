using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public class ProjectionTelemetry : IProjectionTelemetry
{
    public void ProjectionSkipped(
        string projectionName)
    {
        using var x = EventStoreDiagnostics.Source.StartActivity(
            name: $"Projection {projectionName} skipped as no events passed projection filter",
            kind: ActivityKind.Internal,
            tags: new Dictionary<string, object?>
            {
                { EventStoreDiagnostics.TagAttributes.SubscriptionName, projectionName },
            });
    }

    public ProjectionBatchTelemetry StartBatch(
        string projectionName,
        int count)
    {
        var activity = EventStoreDiagnostics.Source.StartActivity(
            name: $"Projection batch {projectionName} started",
            kind: ActivityKind.Internal,
            tags: new Dictionary<string, object?>
            {
                { EventStoreDiagnostics.TagAttributes.SubscriptionName, projectionName },
                { EventStoreDiagnostics.TagAttributes.SubscriptionEventCount, count },
            });

        return new ProjectionBatchTelemetry(activity);
    }
}
