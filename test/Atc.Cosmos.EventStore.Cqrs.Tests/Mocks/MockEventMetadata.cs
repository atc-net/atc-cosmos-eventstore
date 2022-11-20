namespace Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;

public class MockEventMetadata : IEventMetadata
{
    public string EventId { get; set; }

    public string Name { get; set; }

    public string? CorrelationId { get; set; }

    public string? CausationId { get; set; }

    public StreamId StreamId { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public StreamVersion Version { get; set; }
}
