namespace Atc.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Represents an interface for consuming a strongly typed event.
    /// </summary>
    /// <typeparam name="TEvent">Event type to consume.</typeparam>
    public interface IConsumeEvent<in TEvent>
    {
        void Consume(
            TEvent evt,
            EventMetadata metadata);
    }
}