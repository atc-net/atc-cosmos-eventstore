using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal interface IStateProjector<TCommand>
        where TCommand : ICommand
    {
        ValueTask<IEventStreamState> ProjectAsync(
            TCommand command,
            ICommandHandler<TCommand> handler,
            CancellationToken cancellationToken);
    }
}