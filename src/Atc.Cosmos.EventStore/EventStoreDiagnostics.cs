using System.Diagnostics;

namespace Atc.Cosmos.EventStore;

public static class EventStoreDiagnostics
{
    public const string SourceName = "Atc.Cosmos.EventStore";

#pragma warning disable CA1034 // Nested types should not be visible
    public static class TagAttributes
#pragma warning restore CA1034 // Nested types should not be visible
    {
        public const string StreamId = "es.stream_id";
        public const string StreamVersion = "es.stream_version";

        public const string Behavior = "es.command.behavior";
        public const string RequiredVersion = "es.command.required_version";
        public const string BehaviorCount = "es.command.behavior_count";
        public const string CommandId = "es.command.id";
        public const string CorrelationId = "es.command.correlation_id";
        public const string EventCount = "es.command.event_count";

        public const string SubscriptionName = "es.subscription.name";
        public const string SubscriptionInstance = "es.subscription.instance";
        public const string SubscriptionMaxItems = "es.subscription.max_items";
        public const string SubscriptionEventCount = "es.subscription.event_count";

        /// <summary>
        /// Wait time between polling for new events. Values are in seconds.
        /// </summary>
        public const string SubscriptionPollingInterval = "es.subscription.polling_interval";
        public const string Subscription = "es.subscription.";
    }

    internal static readonly ActivitySource Source = new(SourceName, "1.0.0");
}