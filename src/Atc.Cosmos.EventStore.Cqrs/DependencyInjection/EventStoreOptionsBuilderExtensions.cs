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

        builder.Services.AddSingleton(typeof(IStateProjector<>), typeof(StateProjector<>));
        builder.Services.AddSingleton(typeof(IStateWriter<>), typeof(StateWriter<>));
        builder.Services.AddSingleton(typeof(ICommandProcessor<>), typeof(CommandProcessor<>));
        builder.Services.AddSingleton<ICommandProcessorFactory, CommandProcessorFactory>();
        builder.Services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
        builder.Services.AddSingleton<IProjectionOptionsFactory, ProjectionOptionsFactory>();
        builder.Services.AddSingleton<IProjectionFactory, ProjectionFactory>();

        builder.Services.AddSingleton(typeof(ProjectionMetadata<>));

        builder.Services.TryAddSingleton<IProjectionDiagnostics, ProjectionDiagnostics>();
        builder.Services.TryAddSingleton<IProjectionProcessOperation, ProjectionProcessOperation>();

        return builder;
    }
}