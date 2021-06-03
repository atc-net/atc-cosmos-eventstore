using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore
{
    public class EventStoreClientOptions
    {
        public const string CosmosEmulatorConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public string ConnectionString { get; set; } = CosmosEmulatorConnectionString;

        public string EventStoreDatabaseId { get; set; } = "EventStore";

        public string EventStoreContainerId { get; set; } = "event-store";

        public string SubscriptionContainerId { get; set; } = "subscriptions";

        public CosmosClientOptions CosmosClientOptions { get; set; } = new CosmosClientOptions();
    }
}