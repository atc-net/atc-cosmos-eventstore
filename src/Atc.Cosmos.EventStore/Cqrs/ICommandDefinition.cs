using System.Threading;
using System.Threading.Tasks;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public interface ICommandDefinition
    {
        Task<ICommandContext> FromStreamAsync(
            string streamId,
            string? etag,
            string? correlationId = null,
            CancellationToken cancellationToken = default);
    }
}