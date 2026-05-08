namespace Atc.Cosmos.EventStore.Cqrs.Internal;

/// <summary>
/// The default projection factory which just creates projections by
/// getting them from the DI-container.
/// </summary>
internal sealed class DefaultProjectionFactory : IProjectionFactory
{
    private readonly IServiceProvider serviceProvider;

    public DefaultProjectionFactory(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    public Task<IProjection> CreateAsync<TProjection>(
        EventStreamId streamId,
        CancellationToken cancellationToken)
        where TProjection : IProjection
        => Task.FromResult<IProjection>(serviceProvider.GetRequiredService<TProjection>());
}