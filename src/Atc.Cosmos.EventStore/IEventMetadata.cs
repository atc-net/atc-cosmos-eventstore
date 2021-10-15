using System;

namespace Atc.Cosmos.EventStore
{
    /// <summary>
    /// Represents meta-data of an event in a stream.
    /// </summary>
    public interface IEventMetadata
    {
        /// <summary>
        /// Gets the unique id of the event.
        /// </summary>
        string EventId { get; }

        /// <summary>
        /// Gets the correlation id associated with the event.
        /// </summary>
        string? CorrelationId { get; }

        /// <summary>
        /// Gets the causation id associated with the event.
        /// </summary>
        string? CausationId { get; }

        /// <summary>
        /// Gets the id of the stream.
        /// </summary>
        StreamId StreamId { get; }

        /// <summary>
        /// Gets when the event was created.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the event version.
        /// What version this event is within a stream.
        /// </summary>
        StreamVersion Version { get; }
    }
}