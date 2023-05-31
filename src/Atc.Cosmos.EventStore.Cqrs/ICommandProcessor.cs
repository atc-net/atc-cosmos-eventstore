namespace Atc.Cosmos.EventStore.Cqrs;

public interface ICommandProcessor<in TCommand>
    where TCommand : ICommand
{
    ValueTask<CommandResult> ExecuteAsync(
        TCommand command,
        CancellationToken cancellationToken);
}