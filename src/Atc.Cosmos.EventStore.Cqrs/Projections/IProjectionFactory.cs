namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal interface IProjectionFactory
{
    IProjection GetProjection<TProjection>()
        where TProjection : IProjection;

    IProjectionMetadata GetProjectionMetadata<TProjection>()
        where TProjection : IProjection;
}