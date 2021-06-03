using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Events
{
    public abstract class EventDocument : IEvent
    {
        [JsonPropertyName(EventPropertyNames.Id)]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName(EventPropertyNames.PartitionKey)]
        public string PartitionKey { get; set; } = string.Empty;

        [JsonPropertyName(EventPropertyNames.Properties)]
        public EventProperties Properties { get; set; } = new EventProperties();

        [JsonIgnore]
        public virtual object Data { get; set; } = new object();

        [JsonIgnore]
        IEventProperties IEvent.Properties => Properties;
    }
}
