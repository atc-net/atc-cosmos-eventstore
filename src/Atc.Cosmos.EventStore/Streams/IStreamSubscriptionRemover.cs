using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamSubscriptionRemover
    {
        Task DeleteAsync(
            ConsumerGroup consumerGroup,
            CancellationToken cancellationToken);
    }
}