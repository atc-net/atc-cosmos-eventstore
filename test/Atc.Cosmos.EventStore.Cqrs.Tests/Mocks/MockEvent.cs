namespace Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;

public class MockEvent : IEvent
{
    public object Data { get; set; }

    public IEventMetadata Metadata { get; set; }
}
