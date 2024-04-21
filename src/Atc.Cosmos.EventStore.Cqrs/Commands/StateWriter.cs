using Atc.Cosmos.EventStore.Cqrs.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class StateWriter<TCommand> : IStateWriter<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandTelemetry telemetry;
    private readonly IEventStoreClient eventStore;

    public StateWriter(
        ICommandTelemetry telemetry,
        IEventStoreClient eventStore)
    {
        this.telemetry = telemetry;
        this.eventStore = eventStore;
    }

    public ValueTask<CommandResult> WriteEventAsync(
        TCommand command,
        IReadOnlyCollection<object> events,
        CancellationToken cancellationToken)
    {
        var id = command.GetEventStreamId();
        var version = (StreamVersion?)command.RequiredVersion ?? StreamVersion.Any;
        var streamOptions = new StreamWriteOptions
        {
            CausationId = command.CommandId,
            CorrelationId = command.CorrelationId,
        };

        return WriteToEventStoreAsync(
                id.Value,
                version,
                streamOptions,
                events,
                GetRetries(command),
                cancellationToken);
    }

    private static int GetRetries(TCommand command)
        => command.Behavior == OnConflict.Retry
         ? command.BehaviorCount
         : 0;

    private async ValueTask<CommandResult> WriteToEventStoreAsync(
        StreamId id,
        StreamVersion version,
        StreamWriteOptions options,
        IReadOnlyCollection<object> events,
        int retries,
        CancellationToken cancellationToken)
    {
        using var activity = telemetry.WriteToStreamStarted(version, events.Count, retries);

        try
        {
            var response = await eventStore
                .WriteToStreamAsync(
                    id,
                    events,
                    version,
                    options,
                    cancellationToken)
                .ConfigureAwait(false);

            return new CommandResult(id, response.Version, ResultType.Changed);
        }
        catch (StreamWriteConflictException)
        {
            if (retries > 0)
            {
                return await WriteToEventStoreAsync(
                        id,
                        version,
                        options,
                        events,
                        retries - 1,
                        cancellationToken)
                    .ConfigureAwait(false);
            }

            throw;
        }
    }
}