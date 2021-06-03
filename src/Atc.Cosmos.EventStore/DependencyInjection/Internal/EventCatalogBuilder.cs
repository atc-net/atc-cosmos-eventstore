using System;
using System.Collections.Generic;
using Atc.Cosmos.EventStore.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.DependencyInjection.Internal
{
    public class EventCatalogBuilder : IEventCatalogBuilder
    {
        private readonly Dictionary<EventName, Type> mappings = new();

        public IEventCatalogBuilder FromType(string name, Type type)
        {
            mappings.Add(name, type);

            return this;
        }

        public IEventCatalog Build()
            => new EventCatalog(mappings);
    }
}