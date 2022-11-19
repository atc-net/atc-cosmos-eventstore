using System;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.IntegrationTests
{
    public class SampleEventTypeProvider : IEventTypeProvider
    {
        public Type? GetEventType(EventName name) => typeof(SampleEvent);
    }
}
