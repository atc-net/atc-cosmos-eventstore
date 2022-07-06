using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cosmos;
using Atc.Cosmos.EventStore.Cqrs.Commands;
using Atc.Cosmos.EventStore.Cqrs.Internal;
using Atc.Cosmos.EventStore.Cqrs.Projections;
using Atc.Cosmos.EventStore.DependencyInjection;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Atc.Cosmos.EventStore.Cqrs.DependencyInjection.Internal
{
    internal class EventStoreCqrsBuilder : IEventStoreCqrsBuilder
    {
        private readonly EventStoreOptionsBuilder builder;

        public EventStoreCqrsBuilder(
            EventStoreOptionsBuilder builder)
            => this.builder = builder;

        public IEventStoreCqrsBuilder AddCommandsFromAssembly<TAssembly>()
        {
            var commands = typeof(TAssembly)
                .Assembly
                .GetTypes()
                .Where(t => !t.IsAbstract)
                .SelectMany(t => t
                    .GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(ICommandHandler<>)))
                    .Select(i => new
                        {
                            CommandHandlerType = t,
                            CommandType = i.GetGenericArguments()[0],
                        }))
                .Select(t => typeof(EventStoreCqrsBuilder)
                    .GetRuntimeMethods()
                    .First(m => m.Name.Equals(nameof(AddCommand), StringComparison.OrdinalIgnoreCase))
                    .MakeGenericMethod(t.CommandType, t.CommandHandlerType))
                .ToArray();

            foreach (var cmd in commands)
            {
                cmd.Invoke(this, Array.Empty<object>());
            }

            return this;
        }

        public IEventStoreCqrsBuilder AddCommand<TCommand, TCommandHandler>()
            where TCommandHandler : class, ICommandHandler<TCommand>
            where TCommand : ICommand
        {
            builder.Services.AddTransient<ICommandHandler<TCommand>, TCommandHandler>();
            builder.Services.AddSingleton<ICommandHandlerMetadata<TCommand>, CommandHandlerMetadata<TCommand, TCommandHandler>>();

            return this;
        }

        public IEventStoreCqrsBuilder AddProjectionJob<TProjection>(
            string name,
            Action<IProjectionBuilder>? configure = default)
            where TProjection : class, IProjection
        {
            builder.Services.AddHostedService<ProjectionJob<TProjection>>();
            builder.Services.AddSingleton<IProjectionProcessor<TProjection>, ProjectionProcessor<TProjection>>();
            builder.Services.AddTransient<TProjection>();
            builder.Services.TryAddSingleton<IDependencyInitializer>(
                new DependencyInitializer(() => Task.CompletedTask));

            var projectionBuilder = new ProjectionBuilder(name);

            configure?.Invoke(projectionBuilder);

            builder
                .Services
                .Configure<ProjectionOptions>(
                    typeof(TProjection).Name,
                    options => projectionBuilder.Build<TProjection>(options));

            return this;
        }

        public IEventStoreCqrsBuilder AddInitialization(
            int throughput,
            Func<IServiceProvider, Task>? additionInitialization = null)
        {
            builder.Services.RemoveAll<IDependencyInitializer>();
            builder.Services.AddSingleton<IDependencyInitializer>(s =>
                new DependencyInitializer(
                    async () =>
                    {
                        await s.GetRequiredService<IEventStoreInitializer>()
                               .CreateEventStoreAsync(
                                    ThroughputProperties.CreateManualThroughput(throughput),
                                    CancellationToken.None)
                               .ConfigureAwait(false);

                        if (additionInitialization is not null)
                        {
                            await additionInitialization
                                .Invoke(s)
                                .ConfigureAwait(false);
                        }
                    }));
            builder.Services.AddHostedService<DependencyInitializerJob>();

            return this;
        }
    }
}