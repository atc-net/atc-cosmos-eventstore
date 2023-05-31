using Atc.Cosmos.EventStore.Diagnostics;
using Atc.Cosmos.EventStore.Events;
using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.Cosmos;

internal class CosmosSubscriptionFactory : IStreamSubscriptionFactory
{
    private readonly IEventStoreContainerProvider containerProvider;
    private readonly ISubscriptionTelemetry telemetry;

    public CosmosSubscriptionFactory(
        IEventStoreContainerProvider containerProvider,
        ISubscriptionTelemetry telemetry)
    {
        this.containerProvider = containerProvider;
        this.telemetry = telemetry;
    }

    public IStreamSubscription Create(
        ConsumerGroup consumerGroup,
        SubscriptionStartOptions startOptions,
        ProcessEventsHandler eventsHandler,
        ProcessExceptionHandler exceptionHandler)
    {
        var builder = containerProvider
            .GetStreamContainer()
            .GetChangeFeedProcessorBuilder<EventDocument>(
                GetProcessorName(consumerGroup),
                (c, ct) => eventsHandler(c.Where(ExcludeMetaDataChanges).ToArray(), ct))
            .WithErrorNotification((lt, ex) => exceptionHandler(lt, ex))
            .WithLeaseContainer(containerProvider.GetSubscriptionContainer())
            .WithMaxItems(100)
            .WithPollInterval(TimeSpan.FromMilliseconds(1000));

        if (startOptions == SubscriptionStartOptions.FromBegining)
        {
            // Instruct processor to start from beginning.
            // see https://docs.microsoft.com/en-us/azure/cosmos-db/change-feed-processor#reading-from-the-beginning
            builder.WithStartTime(DateTime.MinValue.ToUniversalTime());
        }

        if (!string.IsNullOrEmpty(consumerGroup.Instance))
        {
            builder.WithInstanceName(consumerGroup.Instance);
        }

        return new CosmosSubscriptionProcessor(
            telemetry,
            builder.Build(),
            consumerGroup);
    }

    private static bool ExcludeMetaDataChanges(EventDocument doc)
        => doc is not null;

    private static string GetProcessorName(ConsumerGroup consumerGroup)
        => consumerGroup.Name + ":";
}