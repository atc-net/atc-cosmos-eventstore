using Atc.Cosmos.EventStore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Interface for projecting data from a source to a destination.
/// </summary>
public interface IProjectionBuilder
{
    /// <summary>
    /// Filter on stream id for events projected.
    /// </summary>
    /// <param name="filter">Filter pattern.</param>
    /// <returns>Reference to this <see cref="IProjectionBuilder"/> instance.</returns>
    IProjectionBuilder WithFilter(string filter);

    /// <summary>
    /// Define a job name for the projection.
    /// </summary>
    /// <param name="name">Job name.</param>
    /// <returns>Reference to this <see cref="IProjectionBuilder"/> instance.</returns>
    IProjectionBuilder WithJobName(string name);

    /// <summary>
    /// Set an exception handler for the projection process.
    /// </summary>
    /// <param name="handler">Handler for process exceptions.</param>
    /// <returns>Reference to this <see cref="IProjectionBuilder"/> instance.</returns>
    IProjectionBuilder WithExceptionHandler(ProcessExceptionHandler handler);

    /// <summary>
    /// Indicate the point in time the projection should start from.
    /// </summary>
    /// <param name="startFrom">Projection start options.</param>
    /// <returns>Reference to this <see cref="IProjectionBuilder"/> instance.</returns>
    IProjectionBuilder WithProjectionStartsFrom(SubscriptionStartOptions startFrom);

    /// <summary>
    /// Set the polling interval for the projection.
    /// </summary>
    /// <param name="pollingInterval">Polling interval.</param>
    /// <returns>Reference to this <see cref="IProjectionBuilder"/> instance.</returns>
    IProjectionBuilder WithPollingInterval(TimeSpan pollingInterval);

    /// <summary>
    /// Set maximum number of items for the projection.
    /// </summary>
    /// <param name="maxItems">Maximum items count.</param>
    /// <returns>Reference to this <see cref="IProjectionBuilder"/> instance.</returns>
    IProjectionBuilder WithMaxItems(int maxItems);
}