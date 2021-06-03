using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public interface IEventStoreContainerProvider
    {
        Container GetStreamContainer();

        Container GetSubscriptionContainer();
    }
}