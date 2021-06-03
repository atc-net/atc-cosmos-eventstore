using System.Collections.Generic;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    public class CommandContext : ICommandContext
    {
        private readonly List<object> appliedEvents = new();

        public IReadOnlyCollection<object> Events
            => appliedEvents;

        public void AddEvent(object evt)
            => appliedEvents.Add(evt);

        public object? ResponseObject { get; set; }
    }
}