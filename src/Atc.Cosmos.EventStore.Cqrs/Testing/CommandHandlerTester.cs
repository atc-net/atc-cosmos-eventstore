using Atc.Cosmos.EventStore.Cqrs.Commands;
using Atc.Cosmos.EventStore.Cqrs.Internal;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Cqrs.Testing;

internal class CommandHandlerTester<TCommand> :
    ICommandGiven<TCommand>,
    ICommandWhen<TCommand>,
    ICommandThen
    where TCommand : ICommand
{
    private sealed class TestMetadata :
        ConsumeEventMetadata
    {
        public TestMetadata(object handler)
            : base(handler.GetType())
        {
        }

        public ValueTask ApplyEventAsync(
            IEvent evt,
            object handler,
            CancellationToken cancellationToken)
            => ConsumeAsync(evt, handler, cancellationToken);
    }

    private readonly ICommandHandler<TCommand> handler;
    private readonly TestMetadata handlerMetadata;
    private Func<Task<object[]>> eventsProvider = EmptyEventStreamAsync;
    private TCommand? command;

    public CommandHandlerTester(
        ICommandHandler<TCommand> handler)
    {
        this.handler = handler;
        this.handlerMetadata = new TestMetadata(handler);
    }

    public ICommandWhen<TCommand> GivenEvents(
        Func<Task<object[]>> eventsFactory)
    {
        this.eventsProvider = eventsFactory;

        return this;
    }

    public ICommandThen WhenExecuting(
        TCommand command)
    {
        this.command = command;

        return this;
    }

    public async Task ThenExpectEvents(
        Action<ICommandContextInspector> assert,
        CancellationToken cancellationToken = default)
        => assert.Invoke(
            await ExecuteAsync(
                cancellationToken)
            .ConfigureAwait(false));

    public async Task ThenExpectException<TException>(
        CancellationToken cancellationToken = default)
        where TException : Exception
    {
        try
        {
            await ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"{typeof(TException).Name} not thrown");
    }

    public async Task ThenExpectException<TException>(
        Predicate<TException> predicate,
        CancellationToken cancellationToken = default)
        where TException : Exception
    {
        try
        {
            await ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (TException ex)
         when (predicate(ex))
        {
            return;
        }

        throw new InvalidOperationException($"{typeof(TException).Name} not thrown");
    }

    private static Task<object[]> EmptyEventStreamAsync()
        => Task.FromResult<object[]>(Array.Empty<string>());

    private async Task<CommandContext> ExecuteAsync(CancellationToken cancellationToken)
    {
        var version = 1;
        foreach (var evt in await eventsProvider().ConfigureAwait(false))
        {
            await handlerMetadata
                .ApplyEventAsync(
                    new EventDocument<object>
                    {
                        Data = evt,
                        Properties = new Events.EventMetadata
                        {
                            CausationId = Guid.NewGuid().ToString(),
                            CorrelationId = Guid.NewGuid().ToString(),
                            Name = "test",
                            Version = version++,
                            Timestamp = DateTimeOffset.UtcNow,
                            StreamId = "test-stream",
                        },
                    },
                    handler,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        var context = new CommandContext();

        await handler
            .ExecuteAsync(command!, context, cancellationToken)
            .ConfigureAwait(false);

        return context;
    }
}