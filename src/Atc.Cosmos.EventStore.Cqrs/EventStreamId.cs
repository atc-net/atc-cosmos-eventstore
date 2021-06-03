using System;
using System.Collections.Generic;

namespace Atc.Cosmos.EventStore.Cqrs
{
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

        public IReadOnlyList<string> Parts { get; }

        public string Value { get; }

        public static implicit operator EventStreamId(StreamId id)
            => FromStreamId(id.Value);

        public static EventStreamId FromStreamId(StreamId id)
            => new(
                id.Value.Split(PartSeperator));
    }
}