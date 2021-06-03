using System.Text.Json.Serialization;

namespace Atc.Cosmos.EventStore.Events
{
    public class EventDocument<T> : EventDocument
        where T : class
    {
        public EventDocument()
        {
        }

        public EventDocument(T data, EventProperties properties)
            : base()
        {
            TypedData = data;
            Properties = properties;
        }

        [JsonPropertyName(EventPropertyNames.Data)]
        public T TypedData { get; set; } = default!;

        [JsonIgnore]
        public override object Data
        {
            get { return TypedData; }
            set { TypedData = (T)value; }
        }
    }
}