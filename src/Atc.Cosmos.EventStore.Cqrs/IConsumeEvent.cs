namespace Atc.Cosmos.EventStore.Cqrs;

public interface IConsumeEvent<in TEvent>
{
    void Consume(TEvent evt, EventMetadata metadata);
}