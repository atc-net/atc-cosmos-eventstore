using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    public class StateWriter<TCommand> : IStateWriter<TCommand>
        where TCommand : ICommand
    {
        private readonly IEventStoreClient eventStore;

        public StateWriter(
            IEventStoreClient eventStore)
        {
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
            int reties,
            CancellationToken cancellationToken)
        {
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
                if (reties > 0)
                {
                    return await WriteToEventStoreAsync(
                            id,
                            version,
                            options,
                            events,
                            reties - 1,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                throw;
            }
        }
    }
}