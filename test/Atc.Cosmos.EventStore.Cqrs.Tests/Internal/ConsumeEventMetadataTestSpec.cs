namespace Atc.Cosmos.EventStore.Cqrs.Tests.Internal;

public static class ConsumeEventMetadataTestSpec
{
    public record ConsumedEvent(string Name);

#pragma warning disable S2094 // Classes should not be empty
    public record NotConsumedEvent();
#pragma warning restore S2094 // Classes should not be empty

    public class ConsumesOneEvent
        : IConsumeEvent<ConsumedEvent>
    {
        public EventMetadata? MetadataConsumed { get; set; }

        public ConsumedEvent? EventConsumed { get; set; }

        public void Consume(
            ConsumedEvent evt,
            EventMetadata metadata)
        {
            EventConsumed = evt;
            MetadataConsumed = metadata;
        }
    }

    public class ConsumesOneEventAsync
        : IConsumeEventAsync<ConsumedEvent>
    {
        public EventMetadata? MetadataConsumed { get; set; }

        public ConsumedEvent? EventConsumed { get; set; }

        public Task ConsumeAsync(
            ConsumedEvent evt,
            EventMetadata metadata,
            CancellationToken cancellationToken)
        {
            EventConsumed = evt;
            MetadataConsumed = metadata;

            return Task.CompletedTask;
        }
    }

    public class ConsumesAnyEvent
        : IConsumeAnyEvent
    {
        public EventMetadata? MetadataConsumed { get; set; }

        public object? EventConsumed { get; set; }

        public void Consume(
            object evt,
            EventMetadata metadata)
        {
            EventConsumed = evt;
            MetadataConsumed = metadata;
        }
    }

    public class ConsumesAnyEventAsync
        : IConsumeAnyEventAsync
    {
        public EventMetadata? MetadataConsumed { get; set; }

        public object? EventConsumed { get; set; }

        public Task ConsumeAsync(
            object evt,
            EventMetadata metadata,
            CancellationToken cancellationToken)
        {
            EventConsumed = evt;
            MetadataConsumed = metadata;

            return Task.CompletedTask;
        }
    }
}