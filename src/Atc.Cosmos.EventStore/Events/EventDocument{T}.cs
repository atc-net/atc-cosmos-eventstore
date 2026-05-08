namespace Atc.Cosmos.EventStore.Events;

internal class EventDocument<T> : EventDocument
    where T : class
{
    public EventDocument()
    {
    }

    public EventDocument(
        T data,
        EventMetadata properties)
        : base()
    {
        TypedData = data;
        Properties = properties;
    }

    [JsonPropertyName(EventMetadataNames.Data)]
    public T TypedData { get; set; } = default!;

    [JsonIgnore]
    public override object Data
    {
        get => TypedData;
        set => TypedData = (T)value;
    }
}