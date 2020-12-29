using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos.Fluent;

namespace BigBang.Cosmos.EventStore
{
    public class EventStoreClientOptions
    {
        public string StoreName { get; set; } = string.Empty;

        public int Throughput { get; set; } = 400;

        public IReadOnlyDictionary<Type, string> TypeToNameMapper { get; set; } = new Dictionary<Type, string>();

        public CosmosClientBuilder? CosmosBuilder { get; set; }
    }
}