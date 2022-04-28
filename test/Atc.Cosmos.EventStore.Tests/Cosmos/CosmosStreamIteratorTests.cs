using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Events;
using Atc.Test;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests.Cosmos
{
    public class CosmosStreamIteratorTests
    {
        private readonly FeedResponse<EventDocument> feedResponse;
        private readonly FeedIterator<EventDocument> feedIterator;
        private readonly Container container;
        private readonly IEventStoreContainerProvider containerProvider;

        private readonly IEnumerable<EventDocument<TestEvent>> expectedEvents;
        private readonly CosmosStreamIterator sut;

        private QueryDefinition query = new("SELECT * FROM c");
        private QueryRequestOptions options = new();

        public class TestEvent
        {
            public string Name { get; set; } = string.Empty;
        }

        public CosmosStreamIteratorTests()
        {
            expectedEvents = new List<EventDocument<TestEvent>>
            {
                new EventDocument<TestEvent>(
                    new TestEvent { Name = "test1" },
                    new EventMetadata()),
                new EventDocument<TestEvent>(
                    new TestEvent { Name = "test2" },
                    new EventMetadata()),
                new EventDocument<TestEvent>(
                    new TestEvent { Name = "test3" },
                    new EventMetadata()),
            };
            feedResponse = Substitute.For<FeedResponse<EventDocument>>();
            feedResponse
                .Resource
                .Returns(expectedEvents);

            feedIterator = Substitute.For<FeedIterator<EventDocument>>();
            feedIterator
                .HasMoreResults
                .Returns(returnThis: true, returnThese: false);
            feedIterator
                .ReadNextAsync(default)
                .Returns(feedResponse);

            container = Substitute.For<Container>();
            container
                .GetItemQueryIterator<EventDocument>(
                    Arg.Do<QueryDefinition>(q => query = q),
                    Arg.Any<string>(),
                    Arg.Do<QueryRequestOptions>(o => options = o))
                .ReturnsForAnyArgs(feedIterator);

            containerProvider = Substitute.For<IEventStoreContainerProvider>();
            containerProvider
                .GetStreamContainer()
                .Returns(container, returnThese: null);

            sut = new CosmosStreamIterator(containerProvider);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Iterate_All_Items(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            var received = await ReadStream(streamId, StreamVersion.Any, null, cancellationToken);

            received
                .Should()
                .BeEquivalentTo(expectedEvents);
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Use_PartitionKey(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await ReadStream(streamId, StreamVersion.Any, null, cancellationToken);

            options
                .PartitionKey
                .Should()
                .Be(new PartitionKey(streamId.Value));
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Query_All_When_StreamVersion_Is_Any(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await ReadStream(streamId, StreamVersion.Any, null, cancellationToken);

            query
                .QueryText
                .Should()
                .Be("SELECT * FROM e WHERE e.pk = @partitionKey ORDER BY e.properties.version");
            query
                .GetQueryParameters()
                .Should()
                .HaveCount(1);
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@partitionKey", streamId.Value));
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Query_From_StreamVersion(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await ReadStream(streamId, 10, null, cancellationToken);

            query
                .QueryText
                .Should()
                .Be("SELECT * FROM e WHERE e.pk = @partitionKey AND e.properties.version >= @fromVersion ORDER BY e.properties.version");
            query
                .GetQueryParameters()
                .Should()
                .HaveCount(2);
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@partitionKey", streamId.Value));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@fromVersion", 10L));
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Include_EventName(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await ReadStream(
                streamId,
                10,
                new StreamReadOptions { IncludeEvents = new EventName[] { "evt1" } },
                cancellationToken);

            query
                .QueryText
                .Should()
                .Be("SELECT * FROM e WHERE e.pk = @partitionKey AND e.properties.version >= @fromVersion AND (e.properties.name = @name1) ORDER BY e.properties.version");
            query
                .GetQueryParameters()
                .Should()
                .HaveCount(3);
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@partitionKey", streamId.Value));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@fromVersion", 10L));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@name1", (EventName)"evt1"));
        }

        [Theory, AutoNSubstituteData]
        public async Task Should_Include_Multiple_EventNames(
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            await ReadStream(
                streamId,
                10,
                new StreamReadOptions { IncludeEvents = new EventName[] { "evt1", "evt2", "evt3" } },
                cancellationToken);

            query
                .QueryText
                .Should()
                .Be("SELECT * FROM e WHERE e.pk = @partitionKey AND e.properties.version >= @fromVersion AND (e.properties.name = @name1 OR e.properties.name = @name2 OR e.properties.name = @name3) ORDER BY e.properties.version");
            query
                .GetQueryParameters()
                .Should()
                .HaveCount(5);
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@partitionKey", streamId.Value));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@fromVersion", 10L));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@name1", (EventName)"evt1"));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@name2", (EventName)"evt2"));
            query
                .GetQueryParameters()
                .Should()
                .Contain((Name: "@name3", (EventName)"evt3"));
        }

        private async Task<List<IEvent>> ReadStream(
            StreamId streamId,
            StreamVersion streamVersion,
            StreamReadOptions options,
            CancellationToken cancellationToken)
        {
            var received = new List<IEvent>();
            await foreach (var item in sut.ReadAsync(streamId, streamVersion, options, cancellationToken).ConfigureAwait(false))
            {
                received.Add(item);
            }

            return received;
        }
    }
}