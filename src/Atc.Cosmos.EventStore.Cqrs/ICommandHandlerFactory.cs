namespace Atc.Cosmos.EventStore.Cqrs;

public interface ICommandHandlerFactory
{
    ICommandHandler<TCommand> Create<TCommand>()
        where TCommand : ICommand;
}