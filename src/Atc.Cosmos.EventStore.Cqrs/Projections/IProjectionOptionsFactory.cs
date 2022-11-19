namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal interface IProjectionOptionsFactory
{
    IProjectionOptions GetOptions<TProjection>()
        where TProjection : IProjection;
}