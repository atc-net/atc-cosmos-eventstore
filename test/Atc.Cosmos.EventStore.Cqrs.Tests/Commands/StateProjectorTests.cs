using Atc.Cosmos.EventStore.Cqrs.Commands;
using Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;
using Atc.Cosmos.EventStore.Streams;
using Atc.Test;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Commands;

public class StateProjectorTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_Check_If_Handler_Consumes_Events(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        StateProjector<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        await sut.ProjectAsync(command, handler, cancellationToken);

        handlerMetadata.Received().IsNotConsumingEvents();
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Read_Metadata_For_Handler_With_No_Consumes(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(true);

        await sut.ProjectAsync(command, handler, cancellationToken);

        await eventStore.Received()
            .GetStreamInfoAsync(
                command.GetEventStreamId().Value,
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Validate_Metadata_For_Handler_With_No_Consumes(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        [Frozen, Substitute] IStreamReadValidator validator,
        StateProjector<MockCommand> sut,
        StreamMetadata metadata,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(true);
        eventStore
            .GetStreamInfoAsync(default, default)
            .ReturnsForAnyArgs(metadata);

        await sut.ProjectAsync(command, handler, cancellationToken);

        validator
            .Received(1)
            .Validate(
                metadata,
                command.RequiredVersion!.Value.Value);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Default_To_Any_Version_When_Validating_Metadata(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        [Frozen, Substitute] IStreamReadValidator validator,
        StateProjector<MockCommand> sut,
        StreamMetadata metadata,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        command = command with { RequiredVersion = null };
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(true);
        eventStore
            .GetStreamInfoAsync(default, default)
            .ReturnsForAnyArgs(metadata);

        await sut.ProjectAsync(command, handler, cancellationToken);

        validator
            .Received(1)
            .Validate(
                metadata,
                StreamVersion.Any);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_State_With_Version_From_Metadata_For_Handler_With_No_Consumes(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        StreamMetadata metadata,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(true);
        eventStore
            .GetStreamInfoAsync(default, default)
            .ReturnsForAnyArgs(metadata);

        var result = await sut.ProjectAsync(command, handler, cancellationToken);

        result.Should()
            .BeEquivalentTo(
                new Cqrs.Commands.StreamState()
                {
                    Id = command.GetEventStreamId().Value,
                    Version = metadata.Version,
                });
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Not_Read_Stream_From_Event_Store_For_Handler_With_No_Consumes(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        StreamMetadata metadata,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(true);
        eventStore
            .GetStreamInfoAsync(default, default)
            .ReturnsForAnyArgs(metadata);

        await sut.ProjectAsync(command, handler, cancellationToken);

        _ = eventStore
            .DidNotReceiveWithAnyArgs()
            .ReadFromStreamAsync(
                default,
                default,
                default,
                default);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Read_Stream_From_Event_Store_When_Handler_Consumes(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(false);

        await sut.ProjectAsync(command, handler, cancellationToken);

        _ = eventStore.Received()
            .ReadFromStreamAsync(
                command.GetEventStreamId().Value,
                command.RequiredVersion!.Value.Value,
                filter: null,
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Default_To_Any_Version_When_Reading_Event_Store(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        command = command with { RequiredVersion = null };
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(false);

        await sut.ProjectAsync(command, handler, cancellationToken);

        _ = eventStore.Received()
            .ReadFromStreamAsync(
                command.GetEventStreamId().Value,
                StreamVersion.Any,
                filter: null,
                cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Consume_Events_Found_In_Stream(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        ICollection<MockEvent> events,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        command = command with { RequiredVersion = null };
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(false);
        eventStore
            .ReadFromStreamAsync(
                default,
                default,
                default,
                default)
            .ReturnsForAnyArgs(ToAsyncEnumerable(events));

        await sut.ProjectAsync(command, handler, cancellationToken);

        foreach (var evt in events)
        {
            await handlerMetadata
                .Received()
                .ConsumeAsync(
                    evt,
                    handler,
                    cancellationToken);
        }
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_State_With_Version_From_Last_Event_When_Handler_Consumes(
        [Frozen] ICommandHandlerMetadata<MockCommand> handlerMetadata,
        [Frozen] IEventStoreClient eventStore,
        StateProjector<MockCommand> sut,
        ICollection<MockEvent> events,
        MockEvent lastEvent,
        MockEventMetadata lastEventMetadata,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        CancellationToken cancellationToken)
    {
        handlerMetadata
            .IsNotConsumingEvents()
            .ReturnsForAnyArgs(false);
        lastEvent.Metadata = lastEventMetadata;
        events.Add(lastEvent);
        eventStore
            .ReadFromStreamAsync(
                default,
                default,
                default,
                default)
            .ReturnsForAnyArgs(ToAsyncEnumerable(events));

        var result = await sut.ProjectAsync(
            command,
            handler,
            cancellationToken);

        result.Should()
            .BeEquivalentTo(
                new Cqrs.Commands.StreamState()
                {
                    Id = command.GetEventStreamId().Value,
                    Version = lastEventMetadata.Version,
                });
    }

#pragma warning disable CS1998 // Mark method as async
    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> enumerable)
#pragma warning restore CS1998 // Mark method as async
    {
        foreach (var item in enumerable)
        {
            yield return item;
        }
    }
}