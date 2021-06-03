using System;

namespace Atc.Cosmos.EventStore
{
    public struct EventName : IEquatable<EventName>
    {
        public EventName(string eventName)
        {
            Value = eventName;
        }

        public string Value { get; }

        public static implicit operator EventName(string eventName)
            => new(eventName);

        public static explicit operator string(EventName eventName)
            => eventName.Value;

        public static bool operator ==(EventName left, EventName right)
            => string.Equals(left.Value, right.Value, StringComparison.Ordinal);

        public static bool operator !=(EventName left, EventName right)
            => !string.Equals(left.Value, right.Value, StringComparison.Ordinal);

        public static EventName ToStreamId(string eventName)
            => new(eventName);

        public static string FromEventName(EventName eventName)
            => eventName.Value;

        public static EventName ToEventName(string eventName)
            => new(eventName);

        public static bool Equals(EventName left, EventName right)
            => left.Equals(right);

        public override bool Equals(object? obj)
            => obj is EventName id && Equals(id);

        public bool Equals(EventName other)
            => string.Equals(Value, other.Value, StringComparison.Ordinal);

        public override int GetHashCode()
            => HashCode.Combine(Value);
    }
}