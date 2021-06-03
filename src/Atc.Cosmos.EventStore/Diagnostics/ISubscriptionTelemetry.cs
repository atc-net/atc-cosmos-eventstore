using System;

namespace Atc.Cosmos.EventStore.Diagnostics
{
    public interface ISubscriptionTelemetry
    {
        void ProcessExceptionHandlerFailed(Exception exception, ConsumerGroup consumerGroup);

        ISubscriptionActivity? SubscriptionStarted(ConsumerGroup consumerGroup);
    }
}