using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    internal interface IEventStoreContainerProvider
    {
        Container GetStreamContainer();

        Container GetSubscriptionContainer();

        Container GetIndexContainer();
    }
}