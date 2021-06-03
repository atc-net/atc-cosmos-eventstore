using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Internal
{
    public interface IDependencyInitializer
    {
        Task EnsureInitializeAsync();
    }
}