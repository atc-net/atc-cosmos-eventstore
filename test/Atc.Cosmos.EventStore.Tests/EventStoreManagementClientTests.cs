using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests;

public class EventStoreManagementClientTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_DeleteStream(
        [Frozen] IStreamDeleter deleter,
        EventStoreManagementClient sut,
        StreamId streamId,
        CancellationToken cancellationToken)
    {
        await sut.DeleteStreamAsync(
            streamId,
            cancellationToken: cancellationToken);

        _ = deleter
            .Received(1)
            .DeleteAsync(
                streamId,
                cancellationToken);
    }
}