using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

public sealed class FooEventStreamId : EventStreamId, IEquatable<FooEventStreamId?>
{
    private const string TypeName = "foo";
    public const string FilterIncludeAllEvents = TypeName + ".*";

    public FooEventStreamId(string id)
        : base(TypeName, id)
    {
        Id = id;
    }

    public FooEventStreamId(EventStreamId id)
        : base(id.Parts.ToArray())
    {
        Id = id.Parts[1];
    }

    public string Id { get; }

    public override bool Equals(object? obj)
        => Equals(obj as FooEventStreamId);

    public bool Equals(FooEventStreamId? other)
        => other != null && Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(Value);
}