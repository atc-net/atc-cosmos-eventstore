using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

public sealed class SampleEventStreamId : EventStreamId, IEquatable<SampleEventStreamId?>
{
    private const string TypeName = "sample";
    public const string FilterIncludeAllEvents = TypeName + ".*";

    public SampleEventStreamId(string id)
        : base(TypeName, id)
    {
        Id = id;
    }

    public SampleEventStreamId(EventStreamId id)
        : base(id.Parts.ToArray())
    {
        Id = id.Parts[1];
    }

    public string Id { get; }

    public override bool Equals(object? obj)
        => Equals(obj as SampleEventStreamId);

    public bool Equals(SampleEventStreamId? other)
        => other != null && Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(Value);
}