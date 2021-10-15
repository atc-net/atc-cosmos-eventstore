using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Internal
{
    internal interface IDependencyInitializer
    {
        Task EnsureInitializeAsync();
    }
}