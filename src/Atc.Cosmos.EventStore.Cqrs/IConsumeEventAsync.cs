using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Represents an interface for asynchronous consumption of a strongly typed event.
    /// </summary>
    /// <typeparam name="TEvent">Event type to consume.</typeparam>
    public interface IConsumeEventAsync<in TEvent>
    {
        Task ConsumeAsync(
            TEvent evt,
            EventMetadata metadata,
            CancellationToken cancellationToken);
    }
}