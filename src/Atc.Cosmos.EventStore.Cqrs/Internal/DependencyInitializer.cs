using System;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs.Internal
{
    public class DependencyInitializer : IDependencyInitializer
    {
        private readonly Task initializer;

        public DependencyInitializer(
            Func<Task> task)
        {
            initializer = task();
        }

        public Task EnsureInitializeAsync()
            => initializer;
    }
}