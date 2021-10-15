using System;
using Atc.Cosmos.EventStore.Events;

namespace Atc.Cosmos.EventStore.Cosmos
{
    internal class GuidEventIdProvider : IEventIdProvider
    {
        public string CreateUniqueId(IStreamMetadata metadata)
            => Guid.NewGuid().ToString();
    }
}