namespace Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;

public record MockCommand : ICommand
{
    private readonly EventStreamId eventStreamId = new(Guid.NewGuid().ToString());

    public string CommandId { get; set; }

    public string? CorrelationId { get; set; }

    public EventStreamVersion? RequiredVersion { get; set; }

    public OnConflict Behavior { get; set; }

    public int BehaviorCount { get; set; }

    public EventStreamId GetEventStreamId()
        => eventStreamId;
}