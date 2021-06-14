using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests
{
    public class EventStoreClientTests
    {
        [Theory, AutoNSubstituteData]
        public async ValueTask Should_WriteToStream(
            [Frozen, Substitute] IStreamWriter writer,
            EventStoreClient sut,
            StreamId streamId,
            IReadOnlyList<object> events,
            StreamResponse expected,
            CancellationToken cancellationToken)
        {
            writer
                .WriteAsync(default, default, default, default, default)
                .ReturnsForAnyArgs(Task.FromResult<StreamResponse>(expected));

            var result = await sut.WriteToStreamAsync(
                streamId,
                events,
                StreamVersion.StartOfStream,
                cancellationToken: cancellationToken);

            result
                .Should()
                .BeEquivalentTo(expected);
        }

        [Theory, AutoNSubstituteData]
        public void Should_Throw_When_EventsList_Containes_NullObject(
            EventStoreClient sut,
            StreamId streamId,
            Collection<object> events,
            CancellationToken cancellationToken)
        {
            events.Add(null);

            FluentActions
                .Awaiting(() => sut.WriteToStreamAsync(
                    streamId,
                    events,
                    StreamVersion.StartOfStream,
                    cancellationToken: cancellationToken))
                .Should()
                .Throw<ArgumentException>();
        }
    }
}