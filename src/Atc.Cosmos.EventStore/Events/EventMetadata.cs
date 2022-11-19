using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Events;

/// <summary>
/// Represents a set of properties related to an events.
/// </summary>
internal class EventMetadata : IEventMetadata
{
    [JsonIgnore]
    public string EventId { get; set; } = string.Empty;

    [JsonPropertyName(EventMetadataNames.EventName)]
    public string Name { get; set; } = string.Empty;

    public StreamId StreamId { get; set; } = string.Empty;

    public StreamVersion Version { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public string? CorrelationId { get; set; }

    public string? CausationId { get; set; }
}