namespace Microsoft.Extensions.DependencyInjection;

public interface IEventStoreCqrsBuilder
{
    IEventStoreCqrsBuilder AddInitialization(
        int throughput,
        Func<IServiceProvider, Task>? additionInitialization = null);

    IEventStoreCqrsBuilder AddCommandsFromAssembly<TAssembly>();

    IEventStoreCqrsBuilder AddCommand<TCommand, TCommandHandler>()
        where TCommandHandler : class, ICommandHandler<TCommand>
        where TCommand : ICommand;

    IEventStoreCqrsBuilder AddProjectionJob<TProjection>(
        string name,
        Action<IProjectionBuilder>? configure = null)
        where TProjection : class, IProjection;
}