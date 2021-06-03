using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Diagnostics
{
    public class SubscriptionActivity : ISubscriptionActivity
    {
        private readonly DiagnosticSource source;
        private readonly Activity activity;
        private readonly ConsumerGroup consumerGroup;

        public SubscriptionActivity(
            DiagnosticSource source,
            Activity activity,
            ConsumerGroup consumerGroup)
        {
            this.source = source;
            this.activity = activity;
            this.consumerGroup = consumerGroup;
        }

        public void SubscriptionStopped()
        {
            if (source.IsEnabled(SubscriptionTelemetry.ActivityStartName))
            {
                source.StopActivity(
                    activity,
                    new { ConsumerGroup = consumerGroup });
            }
        }
    }
}