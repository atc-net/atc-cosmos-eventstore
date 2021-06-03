using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        ValueTask ExecuteAsync(
            TCommand command,
            ICommandContext context,
            CancellationToken cancellationToken);
    }
}