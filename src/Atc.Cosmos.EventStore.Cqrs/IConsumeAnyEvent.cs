namespace Atc.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Represents an interface for consuming any kind of event object.
    /// </summary>
    public interface IConsumeAnyEvent
    {
        void Consume(
            object evt,
            EventMetadata metadata);
    }
}