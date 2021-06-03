////using System.Threading;
////using System.Threading.Tasks;
////using Atc.Test;
////using FluentAssertions;
////using Xunit;

////namespace Atc.Cosmos.EventStore.IntegrationTests
////{
////    public class UnitTest1
////    {
////        private const string CosmosEmulatorConnectionString = "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

////        [Trait("Category", "SkipWhenLiveUnitTesting")]
////        [Theory, AutoNSubstituteData]
////        public async Task Should_Write_Events(
////            CancellationToken cancellationToken)
////        {
////            var options = new EventStoreClientOptions
////            {
////                EventTypeProvider = new SampleEventTypeProvider(),
////                EventStoreDatabaseId = "EventStore",
////                EventStoreContainerId = "event-store",
////            };

////            ////await EventStoreInitializer.InitializeAsync(
////            ////    CosmosEmulatorConnectionString,
////            ////    options,
////            ////    cancellationToken);

////            var client = EventStoreFactory.GetEventStore(
////                CosmosEmulatorConnectionString,
////                options);

////            var info = await client.GetStreamInfoAsync(
////                "test-stream/1",
////                cancellationToken);

////            var response = await client.WriteToStreamAsync(
////                "test-stream/1",
////                new[] { new SampleEvent { Name = "Lars" } },
////                StreamVersion.Any,
////                cancellationToken: cancellationToken);

////            response = await client.WriteToStreamAsync(
////                "test-stream/1",
////                new[] { new SampleEvent { Name = "Lars" } },
////                response.Version,
////                cancellationToken: cancellationToken);

////            await client.WriteToStreamAsync(
////                "test-stream/1",
////                new[] { new SampleEvent { Name = "Lars" } },
////                response.Version.Value - 1,
////                cancellationToken: cancellationToken);

////            info
////                .Should()
////                .NotBeNull();
////        }

////        [Trait("Category", "SkipWhenLiveUnitTesting")]
////        [Theory, AutoNSubstituteData]
////        public async Task Should_Read_All_Events(
////            CancellationToken cancellationToken)
////        {
////            var options = new EventStoreClientOptions
////            {
////                EventTypeProvider = new SampleEventTypeProvider(),
////                EventStoreDatabaseId = "EventStore",
////                EventStoreContainerId = "event-store",
////            };

////            var client = EventStoreFactory.GetEventStore(
////                CosmosEmulatorConnectionString,
////                options);

////            var info = await client.GetStreamInfoAsync(
////                "test-stream/1",
////                cancellationToken);

////            long items = 0;
////            await foreach (var evt in client.ReadFromStreamAsync("test-stream/1", cancellationToken: cancellationToken))
////            {
////                items++;
////            }

////            items
////                .Should()
////                .Be(info.Version.Value);

////            items = 0;
////            await foreach (var evt in client.ReadFromStreamAsync("test-stream/1", 12, cancellationToken: cancellationToken))
////            {
////                items++;
////            }

////            items
////                .Should()
////                .Be(0);
////        }
////    }
////}
