using Microsoft.Extensions.Options;

namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    internal class ProjectionOptionsFactory : IProjectionOptionsFactory
    {
        private readonly IOptionsMonitor<ProjectionOptions> namedOptions;

        public ProjectionOptionsFactory(
            IOptionsMonitor<ProjectionOptions> namedOptions)
        {
            this.namedOptions = namedOptions;
        }

        public IProjectionOptions GetOptions<TProjection>()
            where TProjection : IProjection
            => namedOptions.Get(
                typeof(TProjection).Name);
    }
}