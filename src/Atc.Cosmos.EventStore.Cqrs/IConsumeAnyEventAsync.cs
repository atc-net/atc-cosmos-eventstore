using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Represents an interface for asynchronous consumption of any kind of event object.
    /// </summary>
    public interface IConsumeAnyEventAsync
    {
        Task ConsumeAsync(
            object evt,
            EventMetadata metadata,
            CancellationToken cancellationToken);
    }
}