using System;
using Azure.Core;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore
{
    public class EventStoreClientOptions
    {
        public const string EmulatorEndpoint = "https://localhost:8081/";
        public const string EmulatorAuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        [Obsolete("Call UseCosmosEmulator instead.")]
        public const string CosmosEmulatorConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

#pragma warning disable CS0618 // Type or member is obsolete
        private string? connectionString = CosmosEmulatorConnectionString;
#pragma warning restore CS0618 // Type or member is obsolete

        [Obsolete("Call UseCredentals instead.")]
        public string? ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = value;
                AuthKey = null;
                Endpoint = null;
                Credential = null;
            }
        }

        public string EventStoreDatabaseId { get; set; } = "EventStore";

        public string EventStoreContainerId { get; set; } = "event-store";

        public string IndexContainerId { get; set; } = "stream-index";

        public string SubscriptionContainerId { get; set; } = "subscriptions";

        public CosmosClientOptions CosmosClientOptions { get; set; } = new CosmosClientOptions();

        public string? Endpoint { get; private set; }

        public string? AuthKey { get; private set; }

        public TokenCredential? Credential { get; private set; }

        public void UseCredentials(
            string endpoint,
            TokenCredential credentials)
        {
            Credential = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            AuthKey = null;
#pragma warning disable CS0618 // Type or member is obsolete
            ConnectionString = null;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void UseCredentials(
            string endpoint,
            string authKey)
        {
            Credential = null;
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            AuthKey = authKey ?? throw new ArgumentNullException(nameof(authKey));
#pragma warning disable CS0618 // Type or member is obsolete
            ConnectionString = null;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void UseCosmosEmulator()
        {
            Credential = null;
            Endpoint = EmulatorEndpoint;
            AuthKey = EmulatorAuthKey;
#pragma warning disable CS0618 // Type or member is obsolete
            ConnectionString = null;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}