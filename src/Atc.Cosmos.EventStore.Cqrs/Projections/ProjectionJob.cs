using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cqrs.Internal;
using Microsoft.Extensions.Hosting;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public class ProjectionJob<TProjection> :
        IHostedService,
        IProjectionJob<TProjection>
        where TProjection : IProjection
    {
        private readonly IStreamSubscription subscription;
        private readonly IDependencyInitializer jobDependencies;
        private readonly IProjectionProcessor<TProjection> processor;

        public ProjectionJob(
            IEventStoreClient client,
            IDependencyInitializer initializer,
            IProjectionOptionsFactory optionsFactory,
            IProjectionProcessor<TProjection> processor)
        {
            this.jobDependencies = initializer;
            this.processor = processor;

            var options = optionsFactory.GetOptions<TProjection>();
            subscription = client.SubscribeToStreams(
                ConsumerGroup.GetAsAutoScalingInstance(options.Name),
                SubscriptionStartOptions.FromBegining,
                OnProcessEventsAsync,
                OnProcessExceptionAsync);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await jobDependencies
                .EnsureInitializeAsync()
                .ConfigureAwait(false);

            await subscription
                .StartAsync()
                .ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => subscription.StopAsync();

        private ValueTask OnProcessExceptionAsync(Exception exception, CancellationToken cancellationToken)
            => new(Task.CompletedTask);

        private ValueTask OnProcessEventsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
            => processor.ProcessBatchAsync(
                events,
                cancellationToken);
    }
}