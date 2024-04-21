namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public interface ICommandTelemetry
{
    ICommandActivity CommandStarted<TCommand>(
        TCommand command)
        where TCommand : ICommand;

    ICommandProjectionActivity ProjectionStarted();

    IDisposable WriteToStreamStarted(
        StreamVersion version,
        int count,
        int retries);
}
