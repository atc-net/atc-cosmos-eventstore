using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Diagnostics;

public class SubscriptionProcessorTelemetry : ISubscriptionProcessorTelemetry
{
    public void ProcessExceptionHandlerFailed(
        Exception exception,
        ConsumerGroup consumerGroup)
    {
        using var activity = EventStoreDiagnostics.Source.StartActivity($"Process exception for [{consumerGroup.Name}]");

        activity?.RecordException(exception);
    }

    public void SubscriptionStarted(
        ConsumerGroup consumerGroup)
    {
        using var activity = EventStoreDiagnostics.Source.StartActivity(
            name: $"Subscription [{consumerGroup.Name}] started",
            kind: ActivityKind.Consumer,
            tags: new Dictionary<string, object?>
            {
                { EventStoreDiagnostics.TagAttributes.SubscriptionName, consumerGroup.Name },
                { EventStoreDiagnostics.TagAttributes.SubscriptionInstance, consumerGroup.Instance },
                { EventStoreDiagnostics.TagAttributes.SubscriptionMaxItems, consumerGroup.MaxItems },
                { EventStoreDiagnostics.TagAttributes.SubscriptionPollingInterval, consumerGroup.PollingInterval.TotalSeconds },
            });
    }

    public void SubscriptionStopped(
        ConsumerGroup consumerGroup)
    {
        using var activity = EventStoreDiagnostics.Source.StartActivity(
            name: $"Subscription [{consumerGroup.Name}] stopped",
            kind: ActivityKind.Consumer);
    }
}
