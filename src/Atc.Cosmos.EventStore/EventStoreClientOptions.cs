using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Atc.Cosmos.EventStore.Converters;
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

        [Obsolete("Call UseCredentials instead.")]
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

        /// <summary>
        /// Gets collections of custom <seealso cref="JsonConverter"/>.
        /// </summary>
        public ICollection<JsonConverter> CustomJsonConverter { get; } = new List<JsonConverter>();

        /// <summary>
        /// Gets collections of custom <seealso cref="IEventDataConverter"/> event data converters.
        /// </summary>
        public ICollection<IEventDataConverter> EventDataConverter { get; } = new List<IEventDataConverter>();

        public string? Endpoint { get; private set; }

        public string? AuthKey { get; private set; }

        public TokenCredential? Credential { get; private set; }

        /// <summary>
        /// Configure event store to use <seealso cref="TokenCredential"/> instead <see cref="AuthKey"/>.
        /// </summary>
        /// <param name="endpoint">Cosmos account endpoint.</param>
        /// <param name="credentials">Token credentials to use when connecting to cosmos.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="credentials"/> or <paramref name="endpoint"/> are null.</exception>
        public void UseCredentials(
            string endpoint,
            TokenCredential credentials)
        {
            Credential = credentials ?? throw new ArgumentNullException(nameof(credentials));
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            AuthKey = null;
#pragma warning disable CS0618 // Type or member is obsolete
            connectionString = null;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Configure event storte to use AuthKey when connecting to cosmos db.
        /// </summary>
        /// <param name="endpoint">Cosmos account endpoint.</param>
        /// <param name="authKey">Authorization key to connect with.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="authKey"/> or <paramref name="endpoint"/> are null.</exception>
        public void UseCredentials(
            string endpoint,
            string authKey)
        {
            Credential = null;
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            AuthKey = authKey ?? throw new ArgumentNullException(nameof(authKey));
#pragma warning disable CS0618 // Type or member is obsolete
            connectionString = null;
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Configure event store to use cosmos emulator.
        /// </summary>
        public void UseCosmosEmulator()
        {
            Credential = null;
            Endpoint = EmulatorEndpoint;
            AuthKey = EmulatorAuthKey;
#pragma warning disable CS0618 // Type or member is obsolete
            connectionString = null;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}