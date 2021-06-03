using System;
using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Events
{
    /// <summary>
    /// Represents a set of properties related to an events.
    /// </summary>
    public class EventProperties : IEventProperties
    {
        [JsonIgnore]
        public string EventId { get; set; } = string.Empty;

        [JsonPropertyName(EventPropertyNames.EventName)]
        public string Name { get; set; } = string.Empty;

        public StreamId StreamId { get; set; } = string.Empty;

        public StreamVersion Version { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string? CorrelationId { get; set; }

        public string? CausationId { get; set; }

        ////[JsonIgnore]
        ////StreamId IEventProperties.StreamId => StreamId;

        ////[JsonIgnore]
        ////StreamVersion IEventProperties.Version => Version;
    }
}