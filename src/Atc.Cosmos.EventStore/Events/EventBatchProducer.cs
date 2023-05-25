using Atc.Cosmos.EventStore.Streams;

namespace Atc.Cosmos.EventStore.Events;

internal class EventBatchProducer : IEventBatchProducer
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly IEventNameProvider nameProvider;

    public EventBatchProducer(
        IDateTimeProvider dateTimeProvider,
        IEventNameProvider nameProvider)
    {
        this.dateTimeProvider = dateTimeProvider;
        this.nameProvider = nameProvider;
    }

    public StreamBatch FromEvents(
        IReadOnlyCollection<object> events,
        IStreamMetadata metadata,
        StreamWriteOptions? options)
    {
        var timestamp = dateTimeProvider.GetDateTime();
        var version = metadata.Version.Value;

        var documents = events
            .Select(evt => Convert(
                evt,
                metadata,
                ++version, // increment version for event
                options?.CorrelationId,
                options?.CausationId,
                timestamp))
            .ToArray();

        return new StreamBatch(
            new StreamMetadata(
                StreamMetadata.StreamMetadataId,
                metadata.StreamId.Value,
                metadata.StreamId,
                version,
                StreamState.Active,
                timestamp)
            {
                ETag = metadata.ETag,
            },
            documents);
    }

    private EventDocument Convert(
        object evt,
        IStreamMetadata metadata,
        long version,
        string? correlationId,
        string? causationId,
        DateTimeOffset timestamp)
    {
        var streamId = metadata.StreamId.Value;
        var name = nameProvider.GetName(evt);

        return new EventDocument<object>
        {
            Id = $"{version}",
            PartitionKey = streamId,
            Data = evt,
            Properties = new EventMetadata
            {
                CausationId = causationId,
                CorrelationId = correlationId,
                StreamId = streamId,
                Version = version,
                Timestamp = timestamp,
                Name = name,
            },
        };
    }
}