using System.Diagnostics;
using Atc.Cosmos.EventStore.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

internal sealed class ProjectionProcessOperationTelemetry
    : IProjectionProcessOperationTelemetry
{
    private readonly Activity? activity;

    public ProjectionProcessOperationTelemetry(
        Activity? activity)
        => this.activity = activity;

    public void Dispose()
        => activity?.Dispose();

    public void ProjectionCompleted()
    {
        activity?.SetStatus(ActivityStatusCode.Ok, "Completed");
        activity?.Stop();
    }

    public void ProjectionFailed(
        Exception exception)
    {
        activity?.RecordException(exception);
        activity?.Stop();
    }

    public void ProjectionSkipped()
    {
        activity?.SetStatus(ActivityStatusCode.Ok, "Skipped because the projection didn't consume any of the events");
        activity?.Stop();
    }
}