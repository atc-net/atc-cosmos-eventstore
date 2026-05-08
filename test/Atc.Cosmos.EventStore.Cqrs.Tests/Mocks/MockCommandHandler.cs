namespace Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;

public sealed class MockCommandHandler : ICommandHandler<MockCommand>
{
    private List<IEvent> events = new();

    public object? ResponseObject { get; set; }

    public void AddEventsToEmit(params IEvent[] eventsToEmit)
    {
        events = events.Concat(eventsToEmit).ToList();
    }

    public ValueTask ExecuteAsync(
        MockCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        foreach (var evt in events)
        {
            context.AddEvent(evt);
        }

        context.ResponseObject = ResponseObject;

        return default;
    }
}