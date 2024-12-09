namespace Atc.Cosmos.EventStore;

internal interface IEventStoreManagementClient
{
    /// <summary>
    /// Mark stream as closed, preventing any further writes.
    /// </summary>
    /// <remarks>
    ///   Set <paramref name="expectedVersion"/> to the current stream version to ensure no new events has been written to the
    ///   stream prior to executing the delete operation.
    /// </remarks>
    /// <param name="streamId">Id of the event stream to delete.</param>
    /// <param name="expectedVersion">(Optional) Specify the expected version the stream to be at to allow deletion.</param>
    /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
    /// <returns>Response of the end operation.</returns>
    Task<StreamResponse> RetireStreamAsync(
        StreamId streamId,
        StreamVersion? expectedVersion = default,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Purge events from a stream.
    /// </summary>
    /// <remarks>
    ///   Using a negative value for <paramref name="count"/> will purge backwards from <paramref name="version"/>.<br/>
    ///   To purge the entire stream set <paramref name="version"/> to <seealso cref="StreamVersion.StartOfStream"/> and <paramref name="count"/> to 0.<br/>
    ///
    ///   This operation can not be revoked as purged events within the stream will be deleted.
    /// </remarks>
    /// <param name="streamId">Id of the event stream to purge.</param>
    /// <param name="version">Specifies the version of the first event to purge from stream.</param>
    /// <param name="count">Number of events to purge.</param>
    /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task PurgeStreamAsync(
        StreamId streamId,
        StreamVersion version,
        long count,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entire stream.
    /// </summary>
    /// <remarks>Attempting to write to a deleted stream will create a new empty stream.</remarks>
    /// <param name="streamId">Id of the event stream to delete.</param>
    /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DeleteStreamAsync(
        StreamId streamId,
        CancellationToken cancellationToken = default);
}