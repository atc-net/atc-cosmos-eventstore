namespace Atc.Cosmos.EventStore;

public enum SubscriptionStartOptions
{
    /// <summary>
    /// The first time the subscription is started, start receiving changes from the beginning of time.
    /// </summary>
    /// <remarks>
    /// If the subscription has previous been started, it will resume from last checkpoint.
    /// </remarks>
    FromBegining,

    /// <summary>
    /// The first time the subscription is started, start receive changes from now.
    /// </summary>
    /// <remarks>
    /// If the subscription has previous been started, it will resume from last checkpoint.
    /// </remarks>
    FromNow,
}