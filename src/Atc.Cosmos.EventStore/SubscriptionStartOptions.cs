namespace Atc.Cosmos.EventStore;

public readonly struct SubscriptionStartOptions : IEquatable<SubscriptionStartOptions>
{
    /// <summary>
    /// The first time the subscription is started, start receiving changes from the beginning of time.
    /// </summary>
    /// <remarks>
    /// If the subscription has previously been started, it will resume from last checkpoint.
    /// </remarks>
    public static readonly SubscriptionStartOptions FromBeginning = new(DateTime.MinValue);

    /// <summary>
    /// The first time the subscription is started, start receiving changes from now.
    /// </summary>
    /// <remarks>
    /// If the subscription has previously been started, it will resume from last checkpoint.
    /// </remarks>
    public static readonly SubscriptionStartOptions FromNowOrLastCheckpoint = new(DateTime.MaxValue);

    public SubscriptionStartOptions()
        : this(DateTime.MinValue)
    {
    }

    internal SubscriptionStartOptions(DateTime startFrom)
        => StartFrom = startFrom.ToUniversalTime();

    public DateTime StartFrom { get; }

    /// <summary>
    /// Start receiving changes from a given date time.
    /// </summary>
    /// <remarks>
    /// If the subscription has previously been started, it will resume from last checkpoint.
    /// </remarks>
    /// <param name="startFromDate">Date and time to start receiving events from.</param>
    /// <returns>The subscription start options.</returns>
    public static SubscriptionStartOptions FromDateTime(DateTime startFromDate)
        => new(startFromDate);

    public static bool operator ==(SubscriptionStartOptions left, SubscriptionStartOptions right)
        => left.StartFrom == right.StartFrom;

    public static bool operator !=(SubscriptionStartOptions left, SubscriptionStartOptions right)
        => !(left == right);

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override bool Equals(object? obj)
        => obj is SubscriptionStartOptions option && Equals(option);

    public bool Equals(SubscriptionStartOptions other)
        => StartFrom == other.StartFrom;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override int GetHashCode()
        => HashCode.Combine(StartFrom);
}