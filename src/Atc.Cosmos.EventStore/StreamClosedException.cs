using System;
using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore
{
    /// <summary>
    /// Exception is throw when attempting to write to a close stream.
    /// </summary>
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "By Design")]
    public sealed class StreamClosedException : EventStoreException
    {
        private const string MessageText = "Stream is closed";

        public StreamClosedException(StreamId streamId)
            : base(MessageText)
        {
            StreamId = streamId;
        }

        public StreamClosedException(StreamId streamId, Exception innerException)
            : base(MessageText, innerException)
        {
            StreamId = streamId;
        }

        public StreamId StreamId { get; }
    }
}