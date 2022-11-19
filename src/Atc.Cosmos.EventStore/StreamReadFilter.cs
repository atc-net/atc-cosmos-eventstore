namespace Atc.Cosmos.EventStore;

public class StreamReadFilter
{
    /// <summary>
    /// Gets or sets the type of events to read from the stream.
    /// </summary>
    public IReadOnlyCollection<EventName>? IncludeEvents { get; set; }

    /// <summary>
    /// Gets or sets the required version the stream must be at.
    /// </summary>
    public StreamVersion? RequiredVersion { get; set; }
}