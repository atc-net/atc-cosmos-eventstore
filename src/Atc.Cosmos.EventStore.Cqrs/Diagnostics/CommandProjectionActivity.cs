using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public sealed class CommandProjectionActivity
    : ICommandProjectionActivity
{
    private readonly Activity? activity;

    public CommandProjectionActivity(
        Activity? activity)
        => this.activity = activity;

    public void Completed(StreamVersion version)
    {
        activity?.AddTag(EventStoreDiagnostics.TagAttributes.StreamVersion, $"{(long)version}");
        activity?.SetStatus(ActivityStatusCode.Ok);
        activity?.Stop();
    }

    public void Dispose()
    {
        activity?.Stop();
        activity?.Dispose();
    }
}
