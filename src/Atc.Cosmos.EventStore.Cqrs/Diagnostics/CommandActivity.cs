using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

internal sealed class CommandActivity : ICommandActivity
{
    private readonly Activity? activity;

    public CommandActivity(
        Activity? activity)
        => this.activity = activity;

    public void Changed()
    {
        activity?.SetStatus(ActivityStatusCode.Ok, "Changed");
        activity?.Stop();
    }

    public void Conflict()
    {
        activity?.SetStatus(ActivityStatusCode.Error, "Conflict");
        activity?.Stop();
    }

    public void Dispose()
        => activity?.Dispose();

    public void NotModified()
    {
        activity?.SetStatus(ActivityStatusCode.Ok, "NotModified");
        activity?.Stop();
    }
}
