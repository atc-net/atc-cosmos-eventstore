using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal class CommandProcessor<TCommand> : ICommandProcessor<TCommand>
        where TCommand : ICommand
    {
        private readonly IStateWriter<TCommand> stateWriter;
        private readonly IStateProjector<TCommand> stateProjector;
        private readonly ICommandHandler<TCommand> handler;
        private int reruns;

        public CommandProcessor(
            IStateWriter<TCommand> stateWriter,
            IStateProjector<TCommand> stateProjector,
            ICommandHandler<TCommand> handler)
        {
            this.stateWriter = stateWriter;
            this.stateProjector = stateProjector;
            this.handler = handler;
        }

        public async ValueTask<CommandResult> ExecuteAsync(
            TCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                reruns = GetReruns(command);

                // Read and project events to aggregate (command handler).
                var state = await stateProjector
                    .ProjectAsync(command, handler, cancellationToken)
                    .ConfigureAwait(false);

                // Execute command on aggregate.
                var context = new CommandContext(state);
                await handler
                    .ExecuteAsync(command, context, cancellationToken)
                    .ConfigureAwait(false);

                if (context.Events.Count == 0)
                {
                    // Command did not yield any events
                    return new CommandResult(
                        state.Id,
                        state.Version.Value,
                        ResultType.NotModified,
                        context.ResponseObject);
                }

                // Write context to stream.
                var result = await stateWriter
                    .WriteEventAsync(command, context.Events, cancellationToken)
                    .ConfigureAwait(false);

                return new CommandResult(
                    result.Id,
                    result.Version,
                    ResultType.Changed,
                    context.ResponseObject);
            }
            catch (StreamWriteConflictException conflict)
            {
                if (ShouldRerunCommand())
                {
                    return await
                        ExecuteAsync(command, cancellationToken)
                       .ConfigureAwait(false);
                }

                return new CommandResult(
                    conflict.StreamId,
                    conflict.Version,
                    ResultType.Conflict);
            }
            catch (StreamVersionConflictException versionConflict)
            {
                return new CommandResult(
                    versionConflict.StreamId,
                    versionConflict.Version,
                    GetResultType(versionConflict));
            }
        }

        private static int GetReruns(TCommand command)
            => command.Behavior == OnConflict.RerunCommand
             ? command.BehaviorCount
             : 0;

        private static ResultType GetResultType(StreamVersionConflictException versionConflict)
            => versionConflict.Reason switch
            {
                StreamConflictReason.StreamIsEmpty => ResultType.NotFound,
                StreamConflictReason.StreamIsNotEmpty => ResultType.Exists,
                _ => ResultType.Conflict,
            };

        private bool ShouldRerunCommand()
            => reruns-- > 0;
    }
}