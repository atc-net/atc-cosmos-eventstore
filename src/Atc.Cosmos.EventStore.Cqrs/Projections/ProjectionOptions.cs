namespace Atc.Cosmos.EventStore.Cqrs.Projections;

internal class ProjectionOptions : IProjectionOptions
{
    public static readonly ProcessExceptionHandler EmptyExceptionHandler = (e, ct)
        => Task.CompletedTask;

    public ProjectionOptions()
    {
        Name = string.Empty;
        Filters = Array.Empty<ProjectionFilter>();
        ExceptionHandler = EmptyExceptionHandler;
        StartsFrom = SubscriptionStartOptions.FromNowOrLastCheckpoint;
        ShareProjectionAcrossProcesses = true;
    }

    public string Name { get; set; }

    public IReadOnlyCollection<ProjectionFilter> Filters { get; set; }

    public ProcessExceptionHandler ExceptionHandler { get; set; }

    public SubscriptionStartOptions StartsFrom { get; set; }

    public TimeSpan PollingInterval { get; set; }

    public int MaxItems { get; set; }

    public bool ShareProjectionAcrossProcesses { get; set; }

    public ConsumerGroup CreateConsumerGroup()
    {
        var cg = new ConsumerGroup(Name)
        {
            MaxItems = MaxItems,
            PollingInterval = PollingInterval,
            StartOptions = StartsFrom,
        };

        if (ShareProjectionAcrossProcesses)
        {
            cg.Instance = ConsumerGroup.GetAutoScalingInstance();
        }

        return cg;
    }
}