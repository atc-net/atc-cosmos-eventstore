using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Streams;

internal class StreamIndex : IStreamIndex
{
    [JsonPropertyName("id")]
    public StreamId StreamId { get; set; } = string.Empty;

    [JsonPropertyName("pk")]
    public string PartitionKey { get; set; } = nameof(StreamIndex);

    public DateTimeOffset Timestamp { get; set; }

    public bool IsActive { get; set; } = true;
}