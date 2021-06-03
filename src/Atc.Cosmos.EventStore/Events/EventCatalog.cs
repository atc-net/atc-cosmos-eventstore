using System;
using System.Collections.Generic;
using System.Linq;

namespace Atc.Cosmos.EventStore.Events
{
    public class EventCatalog : IEventCatalog
    {
        private readonly IReadOnlyDictionary<EventName, Type> mappings;

        public EventCatalog(IReadOnlyDictionary<EventName, Type> mappings)
        {
            this.mappings = mappings;
        }

        public Type GetEventType(EventName name)
            => mappings.TryGetValue(name, out var type)
             ? type
             : throw new EventNotRegisteredException((string)name);

        public string GetName(object evt)
        {
            try
            {
                return mappings
                    .First(kvp => kvp.Value == evt.GetType())
                    .Key
                    .Value;
            }
            catch (InvalidOperationException ex)
            {
                throw new EventNotRegisteredException(
                    evt.GetType().Name,
                    ex);
            }
        }
    }
}
