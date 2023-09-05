namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal interface IProjectionOptions
{
    string Name { get; }

    IReadOnlyCollection<ProjectionFilter> Filters { get; }

    ProcessExceptionHandler ExceptionHandler { get; }

    SubscriptionStartOptions StartsFrom { get; }

    TimeSpan PollingInterval { get; }

    int MaxItems { get; }

    bool ShareProjectionAcrossProcesses { get; }

    ConsumerGroup CreateConsumerGroup();
}