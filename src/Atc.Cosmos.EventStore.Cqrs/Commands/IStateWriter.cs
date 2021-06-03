using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    public interface IStateWriter<in TCommand>
        where TCommand : ICommand
    {
        ValueTask<CommandResult> WriteEventAsync(
            TCommand command,
            IReadOnlyCollection<object> events,
            CancellationToken cancellationToken);
    }
}