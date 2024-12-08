using System.Collections.ObjectModel;
using Atc.Cosmos.EventStore.Cqrs.Commands;
using Atc.Cosmos.EventStore.Cqrs.Tests.Mocks;
using Atc.Test;
using AutoFixture.Xunit2;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Atc.Cosmos.EventStore.Cqrs.Tests.Commands;

public class CommandProcessorTests
{
    [Theory, AutoNSubstituteData]
    internal async Task Should_Exeute_State_Projector(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);

        await sut.ExecuteAsync(command, cancellationToken);

        await stateProjector.Received(1).ProjectAsync(command, handler, cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Execute_Command(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);

        await sut.ExecuteAsync(command, cancellationToken);

        await handler.Received(1).ExecuteAsync(command, Arg.Any<CommandContext>(), cancellationToken);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Set_Command_Context_StreamVersion(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        ICommandHandler<MockCommand> handler,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);

        await sut.ExecuteAsync(command, cancellationToken);

        var commandContext = handler.ReceivedCallWithArgument<CommandContext>();
        commandContext.StreamVersion.Should().Be(streamState.Version);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_NotModified_When_Command_Emits_No_Events(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        MockCommandHandler handler,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);

        var result = await sut.ExecuteAsync(command, cancellationToken);

        result.Result.Should().Be(ResultType.NotModified);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Write_ResposeObject_To_CommandResult_When_Command_Emits_No_Events(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        MockCommandHandler handler,
        object responseObject,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);
        handler.ResponseObject = responseObject;

        var result = await sut.ExecuteAsync(command, cancellationToken);

        result.Response.Should().Be(responseObject);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Call_StateWriter_With_Events__When_Command_Emits_Events(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        [Frozen] IStateWriter<MockCommand> stateWriter,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        CommandResult commandResult,
        MockCommandHandler handler,
        MockEvent[] events,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);
        stateWriter.WriteEventAsync(command, events, cancellationToken).ReturnsForAnyArgs(commandResult);
        handler.AddEventsToEmit(events);

        await sut.ExecuteAsync(command, cancellationToken);

        await stateWriter.Received(1).WriteEventAsync(command, Arg.Any<IReadOnlyCollection<object>>(), cancellationToken);
        var writtenEvents = stateWriter.ReceivedCallWithArgument<IReadOnlyCollection<object>>();
        writtenEvents.Should().HaveSameCount(events);
        writtenEvents.AsEnumerable().Should().BeEquivalentTo(events);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Return_Changed_When_Command_Emits_Events(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        [Frozen] IStateWriter<MockCommand> stateWriter,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        MockCommandHandler handler,
        CommandResult commandResult,
        MockEvent[] events,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);
        stateWriter.WriteEventAsync(command, events, cancellationToken).ReturnsForAnyArgs(commandResult);
        handler.AddEventsToEmit(events);

        var result = await sut.ExecuteAsync(command, cancellationToken);

        result.Result.Should().Be(ResultType.Changed);
    }

    [Theory, AutoNSubstituteData]
    internal async Task Should_Write_ResposeObject_To_CommandResult_When_Command_Emits_Events(
        [Frozen] ICommandHandlerFactory commandHandlerFactory,
        [Frozen] IStateProjector<MockCommand> stateProjector,
        [Frozen] IStateWriter<MockCommand> stateWriter,
        CommandProcessor<MockCommand> sut,
        MockCommand command,
        MockCommandHandler handler,
        CommandResult commandResult,
        MockEvent[] events,
        object responseObject,
        Atc.Cosmos.EventStore.Cqrs.Commands.StreamState streamState,
        CancellationToken cancellationToken)
    {
        commandHandlerFactory.Create<MockCommand>().Returns(handler);
        stateProjector.ProjectAsync(command, handler, cancellationToken).Returns(streamState);
        stateWriter.WriteEventAsync(command, events, cancellationToken).ReturnsForAnyArgs(commandResult);
        handler.AddEventsToEmit(events);
        handler.ResponseObject = responseObject;

        var result = await sut.ExecuteAsync(command, cancellationToken);

        result.Response.Should().Be(responseObject);
    }
}