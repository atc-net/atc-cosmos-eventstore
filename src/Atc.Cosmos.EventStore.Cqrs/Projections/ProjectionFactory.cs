using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal class ProjectionFactory : IProjectionFactory
{
    private readonly IServiceProvider serviceProvider;

    public ProjectionFactory(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    public IProjection GetProjection<TProjection>()
        where TProjection : IProjection
        => serviceProvider.GetRequiredService<TProjection>();

    public IProjectionMetadata GetProjectionMetadata<TProjection>()
        where TProjection : IProjection
        => serviceProvider.GetRequiredService<ProjectionMetadata<TProjection>>();

    public ProjectionFactory CreateScope()
    {
        return new ProjectionFactory(serviceProvider.CreateScope().ServiceProvider);
    }
}