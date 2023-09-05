using Atc.Cosmos.EventStore.Cqrs.Internal;
using Microsoft.Extensions.Hosting;

namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal class ProjectionJob<TProjection> :
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
            options.CreateConsumerGroup(),
            OnProcessEventsAsync,
            options.ExceptionHandler);
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

    private async Task OnProcessEventsAsync(
        IEnumerable<IEvent> events,
        CancellationToken cancellationToken)
    {
        var action = await processor
            .ProcessBatchAsync(
                events,
                cancellationToken)
            .ConfigureAwait(false);

        if (action == ProjectionAction.Stop)
        {
            await subscription
                .StopAsync()
                .ConfigureAwait(false);
        }
    }
}