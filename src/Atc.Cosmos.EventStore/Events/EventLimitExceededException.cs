using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore.Events;

[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
[SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "By Design")]
public class EventLimitExceededException : EventStoreException
{
    public EventLimitExceededException(int count, int limit)
        : base($"The maximum number of events ({limit}) per command has been exceeded: {count}")
    {
    }
}