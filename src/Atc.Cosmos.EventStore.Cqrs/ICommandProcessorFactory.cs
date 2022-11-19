namespace Atc.Cosmos.EventStore.Cqrs;

public interface ICommandProcessorFactory
{
    ICommandProcessor<TCommand> Create<TCommand>()
        where TCommand : ICommand;
}