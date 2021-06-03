using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Streams
{
    public interface IStreamSubscriptionRemover
    {
        ValueTask DeleteAsync(
            ConsumerGroup consumerGroup,
            CancellationToken cancellationToken);
    }
}