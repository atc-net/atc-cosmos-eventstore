using System.Text.Json.Serialization;

namespace BigBang.Cosmos.EventStore
{
    public abstract class Event
    {
        /// <summary>
        /// Gets or sets unique id of the event.
        /// This is a composite key consisting of {guid} + {Properties.Version}
        /// providing a unique key constraint on inserting document.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Id)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the partition key.
        /// Identifies the value that the event is partitioned by.
        /// This is the same value as this.Properties.Id.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.PartitionKey)]
        public string PartitionKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets properties (meta-data) for the event.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Properties)]
        public EventProperties Properties { get; set; } = new EventProperties();

        /// <summary>
        /// Gets or sets event data object.
        /// </summary>
        [JsonIgnore]
        public virtual object Data { get; set; } = new object();
    }
}
