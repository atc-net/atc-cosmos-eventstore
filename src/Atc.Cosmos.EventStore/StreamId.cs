using System.ComponentModel;

namespace Atc.Cosmos.EventStore;

public struct StreamId : IEquatable<StreamId>
{
    public StreamId(string streamId)
    {
        Value = streamId;
    }

    public string Value { get; }

    public static implicit operator StreamId(string id)
        => new(id);

    public static explicit operator string(StreamId streamId)
        => streamId.Value;

    public static bool operator ==(StreamId left, StreamId right)
        => string.Equals(left.Value, right.Value, StringComparison.Ordinal);

    public static bool operator !=(StreamId left, StreamId right)
        => !string.Equals(left.Value, right.Value, StringComparison.Ordinal);

    public static StreamId ToStreamId(string streamId)
        => new(streamId);

    public static string FromStreamId(StreamId streamId)
        => streamId.Value;

    public static bool Equals(StreamId left, StreamId right)
        => left.Equals(right);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
        => obj is StreamId id && Equals(id);

    public bool Equals(StreamId other)
        => string.Equals(Value, other.Value, StringComparison.Ordinal);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
        => HashCode.Combine(Value);
}