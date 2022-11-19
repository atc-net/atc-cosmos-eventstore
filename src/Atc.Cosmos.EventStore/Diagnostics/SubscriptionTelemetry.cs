using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Diagnostics;

/// <summary>
/// Diagnostic instrumentation for event subscription.
/// </summary>
internal class SubscriptionTelemetry : ISubscriptionTelemetry
{
    public const string DiagnosticListenerName = "Atc.Cosmos.EventStore.Subscription.Source";
    public const string ActivityName = "Atc.Cosmos.EventStore.Subscription";
    public const string ActivityStartName = ActivityName + ".Start";
    public const string ActivityExceptionName = ActivityName + ".Exception";

    private static readonly DiagnosticSource SubscriptionSource
        = new DiagnosticListener(DiagnosticListenerName);

    public void ProcessExceptionHandlerFailed(Exception exception, ConsumerGroup consumerGroup)
    {
        if (SubscriptionSource.IsEnabled(ActivityExceptionName))
        {
            SubscriptionSource
                .Write(
                    ActivityExceptionName,
                    new
                    {
                        Exception = exception,
                        ConsumerGroup = consumerGroup,
                    });
        }
    }

    public ISubscriptionActivity? SubscriptionStarted(ConsumerGroup consumerGroup)
    {
        if (!SubscriptionSource.IsEnabled(ActivityStartName))
        {
            return null;
        }

        var activity = new Activity(ActivityStartName);

        SubscriptionSource
            .StartActivity(
                activity,
                new { ConsumerGroup = consumerGroup });

        return new SubscriptionActivity(
            SubscriptionSource,
            activity,
            consumerGroup);
    }
}