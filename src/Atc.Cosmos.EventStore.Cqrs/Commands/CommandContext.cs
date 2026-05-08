namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class CommandContext : ICommandContext, ICommandContextInspector
{
    public StreamVersion StreamVersion { get; }

    public const int EventLimit = 10;

    private readonly List<object> appliedEvents = [];

    public CommandContext(StreamVersion streamVersion)
        => StreamVersion = streamVersion;

    public IReadOnlyCollection<object> Events
        => appliedEvents;

    public void AddEvent(object evt)
    {
        appliedEvents.Add(evt);
        appliedEvents.ThrowIfEventLimitExceeded(EventLimit);
    }

    public object? ResponseObject { get; set; }
}