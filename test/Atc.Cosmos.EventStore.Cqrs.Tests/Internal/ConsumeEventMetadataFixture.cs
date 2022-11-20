using Atc.Cosmos.EventStore.Cqrs.Internal;
using Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;
using static Atc.Cosmos.EventStore.Cqrs.Tests.Internal.ConsumeEventMetadataTestSpec;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Internal;

public class ConsumeEventMetadataFixture<TProjection> :
    ConsumeEventMetadata
    where TProjection : class, new()
{
    public ConsumeEventMetadataFixture()
        : base(typeof(TProjection))
    {
        Projection = new();
        ConsumableEven = new MockEvent
        {
            Data = new ConsumedEvent("evt"),
            Metadata = new MockEventMetadata
            {
                Name = "name",
                EventId = "eventId",
                StreamId = new StreamId("streamId"),
                Timestamp = DateTimeOffset.Now,
                Version = 1,
            },
        };
        NotConsumableEven = new MockEvent
        {
            Data = new NotConsumedEvent(),
            Metadata = new MockEventMetadata
            {
                Name = "name",
                EventId = "eventId",
                StreamId = new StreamId("streamId"),
                Timestamp = DateTimeOffset.Now,
                Version = 1,
            },
        };
    }

    public TProjection Projection { get; }

    public IEvent ConsumableEven { get; }

    public IEvent NotConsumableEven { get; }

    public async Task<ConsumeEventMetadataFixture<TProjection>> ConsumeEventAsync(
        IEvent evt)
    {
        await ConsumeAsync(evt, Projection, CancellationToken.None);

        return this;
    }
}
