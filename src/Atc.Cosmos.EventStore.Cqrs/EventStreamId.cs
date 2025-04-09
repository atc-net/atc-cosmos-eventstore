namespace Atc.Cosmos.EventStore.Cqrs;

public class EventStreamId
{
    public const string PartSeperator = ".";

    public EventStreamId(params string[] parts)
    {
        if (parts.Length == 0)
        {
            throw new ArgumentException("Please specify one or more parts.", nameof(parts));
        }

        // Validate each part does not include char '.' or is "*"
        Parts = parts;
        Value = string.Join(PartSeperator, parts);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStreamId"/> class from an existing <see cref="EventStreamId"/>.
    /// </summary>
    /// <param name="existing">The existing <see cref="EventStreamId"/>.</param>
    public EventStreamId(EventStreamId existing)
    {
        Parts = existing.Parts;
        Value = existing.Value;
    }

    public IReadOnlyList<string> Parts { get; }

    public string Value { get; }

    public static implicit operator EventStreamId(StreamId id)
        => FromStreamId(id.Value);

    public static EventStreamId FromStreamId(StreamId id)
        => new(
            id.Value.Split(PartSeperator));
}