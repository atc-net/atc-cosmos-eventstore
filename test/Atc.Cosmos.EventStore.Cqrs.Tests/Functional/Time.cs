namespace Atc.Cosmos.EventStore.Cqrs.Tests.Functional;

[ProjectionFilter("*")]
internal class TimeProjection(FakeDatabase fakeDatabase) : IConsumeEvent<TimeTickedEvent>, IProjection
{
    private long lastTick = 0;

    public void Consume(TimeTickedEvent evt, EventMetadata metadata)
    {
        lastTick = evt.TickValue;
    }

    public Task InitializeAsync(EventStreamId id, CancellationToken cancellationToken)
    {
        // Here we could read latest "snapshot/view" value from DB
        return Task.CompletedTask;
    }

    public Task CompleteAsync(CancellationToken cancellationToken)
    {
        // Persist "snapshot/view" value to DB
        fakeDatabase.Save("TimeProjection", lastTick);

        return Task.CompletedTask;
    }

    public Task<ProjectionAction> FailedAsync(Exception exception, CancellationToken cancellationToken)
    {
        return Task.FromResult(ProjectionAction.Continue);
    }
}

/// <summary>
/// Queries all stored TimeTickedEvent.
/// </summary>
internal record QueryTimeTickCommand() : CommandBase<EventStreamId>(new EventStreamId("time"));

/// <summary>
/// Queries all stored TimeTickedEvent - at least one event must exist.
/// </summary>
internal record QueryExistingTimeTickCommand()
    : CommandBase<EventStreamId>(new EventStreamId("time"), RequiredVersion: EventStreamVersion.Exists);

internal class QueryTimeTickHandler :
    ICommandHandler<QueryTimeTickCommand>,
    ICommandHandler<QueryExistingTimeTickCommand>,
    IConsumeEvent<TimeTickedEvent>
{
    private readonly List<(TimeTickedEvent Evt, EventMetadata Metadata)> events = new();

    public void Consume(TimeTickedEvent evt, EventMetadata metadata)
    {
        events.Add((evt, metadata));
    }

    public ValueTask ExecuteAsync(
        QueryTimeTickCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        context.ResponseObject = events;
        return default;
    }

    public ValueTask ExecuteAsync(
        QueryExistingTimeTickCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        context.ResponseObject = events;
        return default;
    }
}

internal record MakeTimeTickCommand(long Tick) : CommandBase<EventStreamId>(new EventStreamId("time"));

internal class MakeTimeTickCommandHandler : ICommandHandler<MakeTimeTickCommand>
{
    public ValueTask ExecuteAsync(
        MakeTimeTickCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        // Add event with Tick value from command
        context.AddEvent(new TimeTickedEvent(command.Tick));
        context.ResponseObject = command.Tick;
        return default;
    }
}

internal sealed record TimeTickedEvent(long TickValue);