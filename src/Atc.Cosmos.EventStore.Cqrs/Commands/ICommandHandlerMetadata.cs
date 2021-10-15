using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal interface ICommandHandlerMetadata<out TCommand>
        where TCommand : ICommand
    {
        bool CanConsumeEvent(IEvent evt);

        bool IsNotConsumingEvents();

        ValueTask ConsumeAsync(
            IEvent evt,
            ICommandHandler<TCommand> handler,
            CancellationToken cancellationToken);
    }
}
