namespace Atc.Cosmos.EventStore.Streams;

internal class StreamWriter : IStreamWriter
{
    private readonly IStreamMetadataReader metadataReader;
    private readonly IStreamWriteValidator validator;
    private readonly IEventBatchProducer batchProducer;
    private readonly IStreamBatchWriter batchWriter;

    public StreamWriter(
        IStreamMetadataReader metadataReader,
        IStreamWriteValidator validator,
        IEventBatchProducer batchProducer,
        IStreamBatchWriter batchWriter)
    {
        this.metadataReader = metadataReader;
        this.validator = validator;
        this.batchProducer = batchProducer;
        this.batchWriter = batchWriter;
    }

    public async Task<StreamResponse> WriteAsync(
        StreamId streamId,
        IReadOnlyCollection<object> events,
        StreamVersion version,
        StreamWriteOptions? options,
        CancellationToken cancellationToken)
    {
        var metadata = await metadataReader
            .GetAsync(streamId, cancellationToken)
            .ConfigureAwait(false);

        validator.Validate(metadata, version);

        var batch = batchProducer
            .FromEvents(events, metadata, options);

        var response = await batchWriter
            .WriteAsync(batch, cancellationToken)
            .ConfigureAwait(false);

        return new StreamResponse(
            response.StreamId,
            response.Version,
            response.Timestamp,
            response.State);
    }
}