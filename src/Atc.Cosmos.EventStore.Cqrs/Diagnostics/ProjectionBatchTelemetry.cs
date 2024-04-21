using System.Diagnostics;
using Atc.Cosmos.EventStore.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public sealed class ProjectionBatchTelemetry : IProjectionBatchTelemetry
{
    private readonly Activity? activity;

    public ProjectionBatchTelemetry(
        Activity? activity)
        => this.activity = activity;

    public void Dispose()
        => activity?.Dispose();

    public IProjectionProcessOperationTelemetry StartProjection(
        StreamId streamId)
    {
        var newActivity = EventStoreDiagnostics.Source.StartActivity(
            name: $"Projection started [{streamId.Value}]",
            kind: ActivityKind.Internal,
            tags: new Dictionary<string, object?>
            {
                { EventStoreDiagnostics.TagAttributes.StreamId, streamId.Value },
            });

        return new ProjectionProcessOperationTelemetry(newActivity);
    }

    public void BatchCompleted()
    {
        activity?.SetStatus(ActivityStatusCode.Ok, "Completed");
        activity?.Stop();
    }

    public void BatchFailed(
        Exception exception)
    {
        activity?.RecordException(exception);
        activity?.Stop();
    }
}