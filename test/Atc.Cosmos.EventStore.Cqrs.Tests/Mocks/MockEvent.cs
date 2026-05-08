namespace Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;

public sealed class MockEvent : IEvent
{
    public object Data { get; set; }

    public IEventMetadata Metadata { get; set; }
}