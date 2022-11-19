namespace Atc.Cosmos.EventStore.Cqrs;

public struct EventStreamVersion : IEquatable<EventStreamVersion>
{
    /// <summary>
    /// Command requires a stream containing zero or more events.
    /// </summary>
    public const long Any = StreamVersion.AnyValue;

    /// <summary>
    /// Command requires a new stream e.g. stream must be empty.
    /// </summary>
    public const long StreamEmpty = StreamVersion.StartOfStreamValue;

    /// <summary>
    /// Command require the stream to exists e.g. stream must contain 1 or more events.
    /// </summary>
    public const long Exists = StreamVersion.NotEmptyValue;

    internal EventStreamVersion(long version)
    {
        // Validate value range
        Value = version;
    }

    /// <summary>
    /// Gets the value of the event stream version.
    /// </summary>
    public long Value { get; }

    public static implicit operator EventStreamVersion(long version)
        => new(version);

    public static explicit operator long(EventStreamVersion version)
        => version.Value;

    public static explicit operator StreamVersion(EventStreamVersion version)
        => version.Value;

    public static EventStreamVersion ToEventStreamVersion(long version)
        => new(version);

    public static StreamVersion ToStreamVersion(EventStreamVersion version)
        => new(version.Value);

    public static long FromEventStreamVersion(EventStreamVersion version)
        => version.Value;

    public override readonly bool Equals(object? obj)
        => Value.Equals(obj);

    public bool Equals(EventStreamVersion other)
        => Value == other.Value;

    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(EventStreamVersion left, EventStreamVersion right)
        => left.Equals(right);

    public static bool operator !=(EventStreamVersion left, EventStreamVersion right)
        => !(left == right);
}