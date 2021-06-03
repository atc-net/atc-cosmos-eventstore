using System;
using System.Threading.Tasks;
using Atc.Cosmos.EventStore.Cqrs;

namespace Microsoft.Extensions.DependencyInjection
{
    public interface IEventStoreCqrsBuilder
    {
        IEventStoreCqrsBuilder AddInitialization(
            int throughput,
            Func<IServiceProvider, Task>? additionInitialization = default);

        IEventStoreCqrsBuilder AddCommandsFromAssembly<TAssembly>();

        IEventStoreCqrsBuilder AddCommand<TCommand, TCommandHandler>()
            where TCommandHandler : class, ICommandHandler<TCommand>
            where TCommand : ICommand;

        IEventStoreCqrsBuilder AddProjectionJob<TProjection>(
            string name,
            Action<IProjectionBuilder>? configure = default)
            where TProjection : class, IProjection;
    }
}