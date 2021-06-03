using System;

namespace Atc.Cosmos.EventStore
{
    public record StreamResponse
    {
        public StreamResponse(
            StreamId streamId,
            StreamVersion version,
            DateTimeOffset timestamp,
            StreamState state)
        {
            StreamId = streamId;
            Version = version;
            Timestamp = timestamp;
            State = state;
        }

        public StreamId StreamId { get; }

        /// <summary>
        /// Gets current version of stream after operation completes.
        /// </summary>
        public StreamVersion Version { get; }

        /// <summary>
        /// Gets state of stream after operation completes.
        /// </summary>
        public StreamState State { get; }

        /// <summary>
        /// Gets time for last update.
        /// </summary>
        public DateTimeOffset Timestamp { get; }
    }
}