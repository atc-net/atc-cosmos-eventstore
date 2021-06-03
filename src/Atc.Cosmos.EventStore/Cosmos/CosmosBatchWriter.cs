using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;
using Microsoft.Azure.Cosmos;

namespace Atc.Cosmos.EventStore.Cosmos
{
    public class CosmosBatchWriter : IStreamBatchWriter
    {
        private readonly IEventStoreContainerProvider containerProvider;

        public CosmosBatchWriter(IEventStoreContainerProvider containerProvider)
        {
            this.containerProvider = containerProvider;
        }

        public async ValueTask<IStreamMetadata> WriteAsync(
            StreamBatch batch,
            CancellationToken cancellationToken)
        {
            var pk = new PartitionKey(batch.Metadata.StreamId.Value);
            var tx = containerProvider
                .GetStreamContainer()
                .CreateTransactionalBatch(pk);

            tx.UpsertItem(
                batch.Metadata,
                new TransactionalBatchItemRequestOptions { IfMatchEtag = batch.Metadata.ETag });

            foreach (var document in batch.Documents)
            {
                tx.CreateItem(
                    document,
                    new TransactionalBatchItemRequestOptions { EnableContentResponseOnWrite = false });
            }

            using var batchResponse = await tx
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            EnsureSuccess(batchResponse, batch.Metadata);

            return GetMetadataFromResponse(batchResponse);
        }

        private static IStreamMetadata GetMetadataFromResponse(TransactionalBatchResponse response)
        {
            var result = response.GetOperationResultAtIndex<StreamMetadata>(0);
            var metadata = result.Resource;

            metadata.ETag = result.ETag;

            return metadata;
        }

        private static void EnsureSuccess(TransactionalBatchResponse response, StreamMetadata metadata)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new StreamWriteConflictException(
                    metadata.StreamId,
                    metadata.Version);
            }
        }
    }
}