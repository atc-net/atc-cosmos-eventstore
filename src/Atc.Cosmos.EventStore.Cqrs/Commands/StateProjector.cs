using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal class StateProjector<TCommand> : IStateProjector<TCommand>
        where TCommand : ICommand
    {
        private readonly IEventStoreClient eventStore;
        private readonly ICommandHandlerMetadata<TCommand> handlerMetadata;

        public StateProjector(
            IEventStoreClient eventStore,
            ICommandHandlerMetadata<TCommand> handlerMetadata)
        {
            this.eventStore = eventStore;
            this.handlerMetadata = handlerMetadata;
        }

        public async ValueTask<IEventStreamState> ProjectAsync(
            TCommand command,
            ICommandHandler<TCommand> handler,
            CancellationToken cancellationToken)
        {
            var state = new StreamState(command.GetEventStreamId());

            await foreach (var evt in eventStore
                .ReadFromStreamAsync(
                    state.Id.Value,
                    (StreamVersion?)command.RequiredVersion ?? StreamVersion.Any,
                    new StreamReadOptions
                    {
                        RequiredVersion = (StreamVersion?)command.RequiredVersion ?? StreamVersion.Any,
                        OnMetadataRead = metadata =>
                        {
                            state.Version = metadata.Version.Value;
                            return handlerMetadata.IsConsumingEvents();
                        },
                    },
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            {
                await handlerMetadata
                    .ConsumeAsync(
                        evt,
                        handler,
                        cancellationToken)
                    .ConfigureAwait(false);

                state.Version = evt.Metadata.Version.Value;
            }

            return state;
        }
    }
}