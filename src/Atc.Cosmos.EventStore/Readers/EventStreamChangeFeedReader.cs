using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Azure.Cosmos;

namespace BigBang.Cosmos.EventStore.Readers
{
    public class EventStreamChangeFeedReader
    {
        private readonly Container container;
        private readonly Container leaseContainer;

        public EventStreamChangeFeedReader(
            Container container,
            Container leaseContainer)
        {
            this.container = container;
            this.leaseContainer = leaseContainer;
        }

        [SuppressMessage("Major Code Smell", "S108:Nested blocks of code should not be left empty", Justification = "By design")]
        public async IAsyncEnumerable<IReadOnlyCollection<Event>> FetchAsync(
            string name,
            string instanceName,
            TimeSpan pollInterval,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Event> changesToProcess = new List<Event>();
            using var changesToProcessSignal = new SemaphoreSlim(0, 1);
            using var changesProcessedSignal = new SemaphoreSlim(0, 1);

            var changeFeedProcessor = container.GetChangeFeedProcessorBuilder<Event>(
                BuildChangeFeedName(name),
                async (changes, cancellationToken) =>
                {
                    if ((changes != null) && changes.Count > 0)
                    {
                        changesToProcess = changes;

                        changesToProcessSignal.Release();

                        try
                        {
                            await changesProcessedSignal
                                .WaitAsync(cancellationToken)
                                .ConfigureAwait(false);
                        }
                        catch (OperationCanceledException)
                        {
                        }
                    }
                })
            .WithInstanceName(instanceName ?? "default")
            .WithPollInterval(pollInterval)
            .WithLeaseContainer(leaseContainer)
            .WithStartTime(DateTime.MinValue.ToUniversalTime())
            .Build();

            await changeFeedProcessor
                .StartAsync()
                .ConfigureAwait(false);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await changesToProcessSignal
                        .WaitAsync(cancellationToken)
                        .ConfigureAwait(false);

                    yield return changesToProcess;

                    changesProcessedSignal.Release();
                }
            }
            finally
            {
                await changeFeedProcessor
                    .StopAsync()
                    .ConfigureAwait(false);
            }
        }

        // We postfix the name so we can find the leasing document to delete
        // when we need to start from beginning of a stream.
        private string BuildChangeFeedName(string name)
            => $"{name}:";
    }
}