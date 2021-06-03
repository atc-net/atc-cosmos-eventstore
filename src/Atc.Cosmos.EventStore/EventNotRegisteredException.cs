using System;
using System.Diagnostics.CodeAnalysis;

namespace Atc.Cosmos.EventStore
{
    [SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
    [SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "By Design")]
    public class EventNotRegisteredException : EventStoreException
    {
        public EventNotRegisteredException(string message)
            : base(message)
        {
        }

        public EventNotRegisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}