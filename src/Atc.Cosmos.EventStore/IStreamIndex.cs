using System;

namespace Atc.Cosmos.EventStore
{
    public interface IStreamIndex
    {
        StreamId StreamId { get; }

        /// <summary>
        /// Gets Timestamp for when the stream was created.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        bool IsActive { get; }
    }
}