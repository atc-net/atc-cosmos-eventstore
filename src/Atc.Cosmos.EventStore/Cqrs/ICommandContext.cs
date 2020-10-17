using System.Threading;
using System.Threading.Tasks;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public interface ICommandContext
    {
        TModel GetProjectionModel<TModel>()
            where TModel : class, new();

        void ApplyEvent<TEvent>(TEvent @event)
            where TEvent : class, new();

        Task<CommandResponse> CommitAsync(CancellationToken cancellationToken = default);
    }
}