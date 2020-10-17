using BigBang.Cosmos.EventStore.Xunit;
using FluentAssertions;
using Xunit;

namespace BigBang.Cosmos.EventStore.Tests
{
    public class EventProducerTests
    {
        [Theory, AutoNSubstituteData]
        public void Should_Produce_PartitionKey(
            string streamId,
            string streamName)
            => EventProducer.GetPartitionKey(streamName, streamId)
                .Should()
                .Be($"{streamName}_{streamId}");
    }
}