using System.Diagnostics;

namespace Atc.Cosmos.EventStore.Diagnostics
{
    internal static class EventStoreDiagnostics
    {
        public static readonly DiagnosticSource SubscriptionListener
            = new DiagnosticListener("Atc.Cosmos.EventStore.Subscription");

        public static readonly DiagnosticSource StreamListener
            = new DiagnosticListener("Atc.Cosmos.EventStore.Stream");
    }
}