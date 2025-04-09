using Atc.Cosmos.EventStore.Cqrs;
using Atc.Cosmos.EventStore.Cqrs.Commands;
using Atc.Cosmos.EventStore.Cqrs.DependencyInjection.Internal;
using Atc.Cosmos.EventStore.Cqrs.Diagnostics;
using Atc.Cosmos.EventStore.Cqrs.Projections;
using Atc.Cosmos.EventStore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class EventStoreOptionsBuilderExtensions
{
    public static EventStoreOptionsBuilder UseCQRS(
        this EventStoreOptionsBuilder builder,
        Action<IEventStoreCqrsBuilder> configure)
    {
        var cqrsBuilder = new EventStoreCqrsBuilder(builder);

        configure?.Invoke(cqrsBuilder);

        builder.Services.TryAddSingleton(typeof(IStateProjector<>), typeof(StateProjector<>));
        builder.Services.TryAddSingleton(typeof(IStateWriter<>), typeof(StateWriter<>));
        builder.Services.TryAddTransient(typeof(ICommandProcessor<>), typeof(CommandProcessor<>));
        builder.Services.TryAddTransient<ICommandProcessorFactory, CommandProcessorFactory>();
        builder.Services.TryAddTransient<ICommandHandlerFactory, CommandHandlerFactory>();
        builder.Services.TryAddSingleton<ICommandTelemetry, CommandTelemetry>();

        builder.Services.TryAddSingleton<IProjectionOptionsFactory, ProjectionOptionsFactory>();

        builder.Services.TryAddSingleton(typeof(ProjectionMetadata<>), typeof(ProjectionMetadata<>));
        builder.Services.TryAddTransient<IProjectionFactory, DefaultProjectionFactory>();

        builder.Services.TryAddSingleton<IProjectionTelemetry, ProjectionTelemetry>();

        return builder;
    }
}