namespace Atc.Cosmos.EventStore;

public class StreamReadFilter
{
    /// <summary>
    /// Gets or sets the type of events to read from the stream.
    /// </summary>
    public IReadOnlyCollection<EventName>? IncludeEvents { get; set; }
}