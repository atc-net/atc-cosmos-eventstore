using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
[SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "By Design")]
public class StreamWriteConflictException : EventStoreException
{
    private const string MessageText = "Stream is not empty";

    public StreamWriteConflictException(StreamId streamId, StreamVersion version, string message)
        : base(message)
    {
        StreamId = streamId;
        Version = version;
    }

    public StreamWriteConflictException(StreamId streamId, StreamVersion version)
        : this(streamId, version, MessageText)
    {
    }

    public StreamWriteConflictException(StreamId streamId, StreamVersion version, Exception innerException)
        : base(MessageText, innerException)
    {
        StreamId = streamId;
        Version = version;
    }

    public StreamId StreamId { get; }

    public StreamVersion Version { get; }
}