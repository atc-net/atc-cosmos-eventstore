namespace Atc.Cosmos.EventStore.Cqrs.Projections
{
    public interface IProjectionOptionsFactory
    {
        IProjectionOptions GetOptions<TProjection>()
            where TProjection : IProjection;
    }
}