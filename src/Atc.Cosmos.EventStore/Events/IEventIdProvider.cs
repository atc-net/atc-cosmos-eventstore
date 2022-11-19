namespace Atc.Cosmos.EventStore.Events;

public interface IEventIdProvider
{
    string CreateUniqueId(IStreamMetadata metadata);
}