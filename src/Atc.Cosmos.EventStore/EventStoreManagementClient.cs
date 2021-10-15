using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore
{
    internal class EventStoreManagementClient : IEventStoreManagementClient
    {
        public Task DeleteStreamAsync(
            StreamId streamId,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task PurgeStreamAsync(
            StreamId streamId,
            StreamVersion version,
            long count,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<StreamResponse> RetireStreamAsync(
            StreamId streamId,
            StreamVersion? expectedVersion = default,
            CancellationToken cancellationToken = default)
            => throw new System.NotImplementedException();
    }
}