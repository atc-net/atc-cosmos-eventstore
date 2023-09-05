namespace Atc.Cosmos.EventStore;

public class ConsumerGroup
{
    private const string DefaultInstance = ".default";
    private const int DefaultMaxItems = 100;
    private static readonly TimeSpan DefaultPollingInterval = TimeSpan.FromSeconds(1);
    private static readonly SubscriptionStartOptions DefaultStartOptions = SubscriptionStartOptions.FromBegining;

    public ConsumerGroup(string name)
    {
        Name = name;
        Instance = DefaultInstance;
        PollingInterval = DefaultPollingInterval;
        MaxItems = DefaultMaxItems;
        StartOptions = DefaultStartOptions;
    }

    public string Name { get; }

    public string Instance { get; set; }

    public TimeSpan PollingInterval { get; set; }

    public int MaxItems { get; set; }

    public SubscriptionStartOptions StartOptions { get; set; }

    public static string GetAutoScalingInstance()
        => Guid.NewGuid().ToString();
}