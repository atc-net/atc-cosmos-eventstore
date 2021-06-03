using System;
using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Streams
{
    public class StreamMetadata : IStreamMetadata
    {
        public const string StreamMetadataId = "meta-data";

        public StreamMetadata(
            string id,
            string partitionKey,
            StreamId streamId,
            StreamVersion version,
            StreamState state,
            DateTimeOffset timestamp)
        {
            Id = id;
            PartitionKey = partitionKey;
            StreamId = streamId;
            Version = version;
            State = state;
            Timestamp = timestamp;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("pk")]
        public string PartitionKey { get; set; }

        public StreamId StreamId { get; set; }

        public StreamVersion Version { get; set; }

        public StreamState State { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [JsonIgnore]
        public string ETag { get; set; } = string.Empty;
    }
}