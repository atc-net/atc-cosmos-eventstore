using System;

namespace BigBang.Cosmos.EventStore.Serialization
{
    public interface IEventTypeNameMapper
    {
        Type GetEventType(string name);

        string GetEventName(Type type);
    }
}