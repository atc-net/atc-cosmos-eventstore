using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public class CommandTelemetry : ICommandTelemetry
{
    public ICommandActivity CommandStarted<TCommand>(
        TCommand command)
        where TCommand : ICommand
    {
        var activity = EventStoreDiagnostics.Source.StartActivity(
            name: $"Execute {typeof(TCommand).Name} {command.GetEventStreamId().Value}",
            kind: ActivityKind.Internal,
            tags: new Dictionary<string, object?>
            {
                { EventStoreDiagnostics.TagAttributes.StreamId, command.GetEventStreamId().Value },
                { EventStoreDiagnostics.TagAttributes.RequiredVersion, (long?)command.RequiredVersion },
                { EventStoreDiagnostics.TagAttributes.Behavior, command.Behavior },
                { EventStoreDiagnostics.TagAttributes.BehaviorCount, command.BehaviorCount },
                { EventStoreDiagnostics.TagAttributes.CommandId, command.CommandId },
                { EventStoreDiagnostics.TagAttributes.CorrelationId, command.CorrelationId },
            });

        return new CommandActivity(activity);
    }

    public ICommandProjectionActivity ProjectionStarted()
    {
        var activity = EventStoreDiagnostics.Source.StartActivity(
            $"Projection events from stream",
            ActivityKind.Internal);

        return new CommandProjectionActivity(activity);
    }

    public IDisposable WriteToStreamStarted(
        StreamVersion version,
        int count,
        int retries)
    {
        var activity = EventStoreDiagnostics.Source.StartActivity(
            name: $"Write events to stream",
            kind: ActivityKind.Internal,
            tags: new Dictionary<string, object?>
            {
                { EventStoreDiagnostics.TagAttributes.EventCount, count },
                { EventStoreDiagnostics.TagAttributes.RequiredVersion, (long)version },
            });

        return new CommandWriterActivity(activity);
    }
}
