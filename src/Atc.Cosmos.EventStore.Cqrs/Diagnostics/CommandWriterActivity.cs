using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

internal sealed class CommandWriterActivity : IDisposable
{
    private readonly Activity? activity;

    public CommandWriterActivity(
        Activity? activity)
        => this.activity = activity;

    public void Dispose()
    {
        activity?.SetStatus(ActivityStatusCode.Ok);
        activity?.Stop();
        activity?.Dispose();
    }
}