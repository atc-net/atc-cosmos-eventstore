using System;
using System.Text.Json.Serialization;

namespace BigBang.Cosmos.EventStore
{
    /// <summary>
    /// Represents a set of properties related to a set of core events for a given aggregate.
    /// </summary>
    public class EventProperties
    {
        /// <summary>
        /// Gets or sets name of the event, used to identify the underlying type of the event.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.EventName)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets ID of stream this event belongs too.
        /// </summary>
        /// <remarks>This is the partition key by which an aggregates events are partitioned in CosmosDb.</remarks>
        [JsonPropertyName(EventPropertyNames.StreamId)]
        public string StreamId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets name of the steam the event belongs too.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.StreamName)]
        public string StreamName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets event version.
        /// What version this event is within an aggregate.
        /// </summary>
        /// <remarks>If stream type is Timeseries the value is number of ticks of the <see cref="Timestamp"/>.</remarks>
        [JsonPropertyName(EventPropertyNames.Version)]
        public long Version { get; set; } = 0;

        /// <summary>
        /// Gets or sets time-stamp for when the event was created.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Timestamp)]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets correlation key used to track a request through various systems and services.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.CorrelationId)]
        public string CorrelationId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets etag.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Etag)]
        public string Etag { get; set; } = string.Empty;
    }
}