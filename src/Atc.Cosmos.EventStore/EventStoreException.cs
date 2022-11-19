using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore;

[SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "By Design")]
public abstract class EventStoreException : Exception
{
    protected EventStoreException()
    {
    }

    protected EventStoreException(string message)
        : base(message)
    {
    }

    protected EventStoreException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}