namespace Atc.Cosmos.EventStore.Events;

public interface IEventTypeProvider
{
    Type? GetEventType(EventName name);
}