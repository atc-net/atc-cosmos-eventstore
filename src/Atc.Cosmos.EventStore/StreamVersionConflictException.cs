using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
[SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "By Design")]
public class StreamVersionConflictException : EventStoreException
{
    public StreamVersionConflictException(
        StreamId streamId,
        StreamVersion version,
        StreamVersion expectedVersion,
        StreamConflictReason reason,
        string message)
        : base(message)
    {
        StreamId = streamId;
        Version = version;
        ExpectedVersion = expectedVersion;
        Reason = reason;
    }

    public StreamId StreamId { get; }

    public StreamVersion Version { get; }

    public StreamVersion ExpectedVersion { get; }

    public StreamConflictReason Reason { get; }
}