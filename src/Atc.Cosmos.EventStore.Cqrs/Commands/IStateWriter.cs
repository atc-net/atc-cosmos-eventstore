namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal interface IStateWriter<in TCommand>
    where TCommand : ICommand
{
    ValueTask<CommandResult> WriteEventAsync(
        TCommand command,
        IReadOnlyCollection<object> events,
        CancellationToken cancellationToken);
}