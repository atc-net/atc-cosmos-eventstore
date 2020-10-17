using System.Text.Json.Serialization;

namespace BigBang.Cosmos.EventStore
{
    public class Event<T> : Event
        where T : class, new()
    {
        public Event() : this (new T(), new EventProperties())
        { }

        public Event(T data, EventProperties properties)
            : base()
        {
            TypedData = data;
            Properties = properties;
        }

        /// <summary>
        /// Event data object.
        /// </summary>
        [JsonPropertyName(EventPropertyNames.Data)]
        public T TypedData { get; set; }

        [JsonIgnore]
        public override object Data
        {
            get { return TypedData; }
            set { TypedData = (T)value; }
        }
    }
}