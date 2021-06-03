namespace Atc.Cosmos.EventStore.Cqrs.Testing
{
    public interface ICommandWhen<in TCommand>
        where TCommand : ICommand
    {
        ICommandThen WhenExecuting(TCommand command);
    }
}