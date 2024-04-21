using Atc.Cosmos.EventStore.Cqrs.Diagnostics;

namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class CommandProcessor<TCommand> : ICommandProcessor<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandTelemetry telemetry;
    private readonly IStateWriter<TCommand> stateWriter;
    private readonly IStateProjector<TCommand> stateProjector;
    private readonly ICommandHandlerFactory handlerFactory;

    public CommandProcessor(
        ICommandTelemetry telemetry,
        IStateWriter<TCommand> stateWriter,
        IStateProjector<TCommand> stateProjector,
        ICommandHandlerFactory handlerFactory)
    {
        this.telemetry = telemetry;
        this.stateWriter = stateWriter;
        this.stateProjector = stateProjector;
        this.handlerFactory = handlerFactory;
    }

    public async ValueTask<CommandResult> ExecuteAsync(
        TCommand command,
        CancellationToken cancellationToken)
        => await SafeExecuteAsync(
                command,
                GetReruns(command),
                cancellationToken)
            .ConfigureAwait(false);

    private static ResultType GetResultType(StreamVersionConflictException versionConflict)
        => versionConflict.Reason switch
        {
            StreamConflictReason.StreamIsEmpty => ResultType.NotFound,
            StreamConflictReason.StreamIsNotEmpty => ResultType.Exists,
            _ => ResultType.Conflict,
        };

    private static int GetReruns(TCommand command)
        => command.Behavior == OnConflict.RerunCommand
         ? command.BehaviorCount
         : 0;

    private async ValueTask<CommandResult> SafeExecuteAsync(
        TCommand command,
        int reruns,
        CancellationToken cancellationToken)
    {
        using var activity = telemetry.CommandStarted(command);
        try
        {
            var handler = handlerFactory.Create<TCommand>();

            // Read and project events to aggregate (command handler).
            var state = await stateProjector
                .ProjectAsync(command, handler, cancellationToken)
                .ConfigureAwait(false);

            // Execute command on aggregate.
            var context = new CommandContext();
            await handler
                .ExecuteAsync(command, context, cancellationToken)
                .ConfigureAwait(false);

            if (context.Events.Count == 0)
            {
                activity.NotModified();

                // Command did not yield any events
                return new CommandResult(
                    state.Id,
                    state.Version,
                    ResultType.NotModified,
                    context.ResponseObject);
            }

            // Write context to stream.
            var result = await stateWriter
                .WriteEventAsync(command, context.Events, cancellationToken)
                .ConfigureAwait(false);

            activity.Changed();

            return new CommandResult(
                result.Id,
                result.Version,
                ResultType.Changed,
                context.ResponseObject);
        }
        catch (StreamWriteConflictException conflict)
        {
            reruns--;
            if (reruns > 0)
            {
                return await
                    SafeExecuteAsync(command, reruns, cancellationToken)
                   .ConfigureAwait(false);
            }

            activity.Conflict();

            return new CommandResult(
                conflict.StreamId,
                conflict.Version,
                ResultType.Conflict);
        }
        catch (StreamVersionConflictException versionConflict)
        {
            activity.Conflict();

            return new CommandResult(
                versionConflict.StreamId,
                versionConflict.Version,
                GetResultType(versionConflict));
        }
    }
}