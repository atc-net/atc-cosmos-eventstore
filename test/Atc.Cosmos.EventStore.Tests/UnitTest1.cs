using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace BigBang.Cosmos.EventStore.Tests
{
    public class UnitTest1
    {
        public const string AccountEndpoint = "https://localhost:8081";
        public const string AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        private Dictionary<Type, string> typeToName = new Dictionary<Type, string>
        {
            { typeof(TestEvent), "test-event" },
        };

        [Fact]
        public async Task Test1()
        {
            // NOTE: For this test to run you need to install CosmosEmulator and run it locally.
            var client = new EventStoreClient(
                AccountEndpoint,
                AuthKey,
                "debug-store",
                5000,
                typeToName);

            await client.InitializeStoreAsync();

            var stream = client.GetVersionedStream("new-stream");
            var eventsToInsert = new List<object>
            {
                new TestEvent { Name = "demo1" },
                new TestEvent { Name = "demo2" },
                new TestEvent { Name = "demo3" },
            };

            var subscription = stream.CreateOrResumeSubscription(
                "read-model-builder",
                StreamUpdated,
                pollingInterval: TimeSpan.FromMilliseconds(500));

            await subscription.StartFromBeginningAsync();
            await Task.Delay(2000);

            ////var response = await stream.WriteEventsAsync("1", eventsToInsert);

            ////await foreach (var events in stream.ReadStreamAsync("1"))
            ////{
            ////    foreach (var @event in events)
            ////    {
            ////        await Task.Delay(500);
            ////        Debug.WriteLine($"ID: {@event.Id}");
            ////    }
            ////}

            await Task.Delay(2000);
            await subscription.StopAsync();

            ////response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        private Task StreamUpdated(IReadOnlyCollection<Event> events, CancellationToken cancellationToken)
        {
            foreach (var @event in events)
            {
                Debug.WriteLine($"Version: {@event.Properties.Version}");
            }

            return Task.CompletedTask;
        }
    }
}