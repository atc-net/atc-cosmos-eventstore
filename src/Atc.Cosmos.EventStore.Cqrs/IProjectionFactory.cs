namespace Atc.Cosmos.EventStore.Cqrs;

/// <summary>
/// Responsible for creating <see cref="IProjection"/> instances.
/// </summary>
public interface IProjectionFactory
{
    /// <summary>
    /// Creates a projection of type <typeparamref name="TProjection"/> for the event stream
    /// identified by <paramref name="streamId"/>.
    /// </summary>
    /// <param name="streamId">ID of the stream being projected.</param>
    /// <param name="cancellationToken">Cancellation.</param>
    /// <typeparam name="TProjection">Type of projection to create.</typeparam>
    /// <returns>The created projection.</returns>
    public Task<IProjection> CreateAsync<TProjection>(
        EventStreamId streamId,
        CancellationToken cancellationToken)
        where TProjection : IProjection;
}