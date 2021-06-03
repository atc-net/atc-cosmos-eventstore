using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Atc.Cosmos.EventStore.Cqrs.Internal
{
    public class DependencyInitializerJob : IHostedService
    {
        private readonly IDependencyInitializer initializer;

        public DependencyInitializerJob(
            IDependencyInitializer initializer)
            => this.initializer = initializer;

        public Task StartAsync(CancellationToken cancellationToken)
            => initializer.EnsureInitializeAsync();

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}