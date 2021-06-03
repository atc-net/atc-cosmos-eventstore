using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore
{
    public interface IEventStoreManagementClient
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
        ValueTask<StreamResponse> RetireStreamAsync(
            string streamId,
            long? expectedVersion = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Purge stream for all events.
        /// </summary>
        /// <remarks>This operation can not be revoked as all events within the stream will be deleted.</remarks>
        /// <param name="streamId">Id of the event stream to purge.</param>
        /// <param name="fromVersion">Specifies the version of the first event to purge from stream.</param>
        /// <param name="count">Number of events to purge.</param>
        /// <param name="cancellationToken">(Optional) <seealso cref="CancellationToken"/> representing request cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        ValueTask PurgeStreamAsync(
            string streamId,
            StreamVersion fromVersion,
            long count,
            CancellationToken cancellationToken = default);

        ValueTask DeleteStreamAsync(
            string streamId,
            CancellationToken cancellationToken = default);
    }
}