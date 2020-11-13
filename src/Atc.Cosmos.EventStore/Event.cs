using System.Text.Json.Serialization;

namespace BigBang.Cosmos.EventStore
{
    public abstract class Event
    {
        /// <summary>
        /// Unique id of the event.
        /// This is a composite key consisting of {guid} + {Properties.Version}
        /// providing a unique key constraint on inserting document.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Id)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Identifies the value that the event is partitioned by.
        /// This is the same value as this.Properties.Id.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.PartitionKey)]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Meta-data for the event.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Properties)]
        public EventProperties Properties { get; set; } = new EventProperties();

        /// <summary>
        /// Event data object.
        /// </summary>
        [JsonIgnore]
        public virtual object Data { get; set; } = new object();
    }
}
