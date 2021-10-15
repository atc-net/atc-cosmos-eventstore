using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Events
{
    internal abstract class EventDocument : IEvent
    {
        [JsonPropertyName(EventMetadataNames.Id)]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName(EventMetadataNames.PartitionKey)]
        public string PartitionKey { get; set; } = string.Empty;

        [JsonPropertyName(EventMetadataNames.Properties)]
        public EventMetadata Properties { get; set; } = new EventMetadata();

        [JsonIgnore]
        public virtual object Data { get; set; } = new object();

        [JsonIgnore]
        IEventMetadata IEvent.Metadata => Properties;
    }
}
