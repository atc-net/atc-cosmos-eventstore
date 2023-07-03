namespace Atc.Cosmos.EventStore.Cqrs.Testing;

public static class CommandHandlerExtensions
{
    public static ICommandWhen<TCommand> GivenStreamContainingEvents<TCommand>(
        this ICommandHandler<TCommand> handler,
        params object[] events)
        where TCommand : ICommand
        => new CommandHandlerTester<TCommand>(handler)
            .GivenEvents(() => Task.FromResult(events));

    public static ICommandWhen<TCommand> GivenStreamContainingEvents<TCommand>(
        this ICommandHandler<TCommand> handler,
        Func<Task<object[]>> eventsFactory)
        where TCommand : ICommand
        => new CommandHandlerTester<TCommand>(handler)
            .GivenEvents(eventsFactory);
}