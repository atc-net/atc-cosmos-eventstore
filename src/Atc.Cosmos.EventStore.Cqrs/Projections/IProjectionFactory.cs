namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public interface IProjectionFactory
    {
        IProjection GetProjection<TProjection>()
            where TProjection : IProjection;

        IProjectionMetadata GetProjectionMetadata<TProjection>()
            where TProjection : IProjection;
    }
}