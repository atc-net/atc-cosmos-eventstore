namespace Atc.Cosmos.EventStore;

public interface IEvent
{
    /// <summary>
    /// Gets event data.
    /// </summary>
    object Data { get; }

    /// <summary>
    /// Gets event metadata.
    /// </summary>
    IEventMetadata Metadata { get; }
}