using System.Collections.Generic;
using Atc.Cosmos.EventStore.Cqrs.Testing;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    internal class CommandContext : ICommandContext, ICommandContextInspector
    {
        private readonly List<object> appliedEvents = new();

        public CommandContext(
            IEventStreamState streamState)
            => StreamState = streamState;

        public IReadOnlyCollection<object> Events
            => appliedEvents;

        public void AddEvent(object evt)
            => appliedEvents.Add(evt);

        public object? ResponseObject { get; set; }

        public IEventStreamState StreamState { get; }
    }
}