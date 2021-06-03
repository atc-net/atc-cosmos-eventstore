using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Diagnostics;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Streams
{
    public class StreamSubscriptionFactory : IStreamSubscriptionFactory
    {
        private readonly ISubscriptionTelemetry telemetry;
        private readonly IStreamSubscriptionProvider subscriptionProvider;

        public StreamSubscriptionFactory(
            ISubscriptionTelemetry telemetry,
            IStreamSubscriptionProvider subscriptionProvider)
        {
            this.telemetry = telemetry;
            this.subscriptionProvider = subscriptionProvider;
        }

        public IStreamSubscription Create(
            ConsumerGroup consumerGroup,
            SubscriptionStartOptions startOptions,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler errorHandler)
            => subscriptionProvider
                .Create(
                    consumerGroup,
                    startOptions,
                    (changes, token) => ProcessEventDocumentAsync(
                        changes,
                        consumerGroup,
                        eventsHandler,
                        errorHandler,
                        token));

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "By Design")]
        private async Task ProcessEventDocumentAsync(
            IReadOnlyCollection<EventDocument> changes,
            ConsumerGroup consumerGroup,
            ProcessEventsHandler eventsHandler,
            ProcessExceptionHandler errorHandler,
            CancellationToken cancellationToken)
        {
            try
            {
                await eventsHandler(
                    changes,
                    cancellationToken)
                .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                try
                {
                    await errorHandler(
                        exception,
                        cancellationToken)
                    .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    telemetry.ProcessExceptionHandlerFailed(ex, consumerGroup);
                }
            }
        }
    }
}