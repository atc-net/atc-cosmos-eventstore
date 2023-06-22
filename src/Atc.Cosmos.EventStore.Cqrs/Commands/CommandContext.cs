using Atc.Cosmos.EventStore.Cqrs.Testing;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class CommandContext : ICommandContext, ICommandContextInspector
{
    private readonly List<object> appliedEvents = new();

    public IReadOnlyCollection<object> Events
        => appliedEvents;

    public void AddEvent(object evt)
    {
        appliedEvents.Add(evt);
        appliedEvents.ThrowIfEventLimitExceeded();
    }

    public object? ResponseObject { get; set; }
}