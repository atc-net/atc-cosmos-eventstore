using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly IServiceProvider provider;

    public CommandHandlerFactory(
        IServiceProvider provider)
        => this.provider = provider;

    public ICommandHandler<TCommand> Create<TCommand>()
        where TCommand : ICommand
        => provider.GetRequiredService<ICommandHandler<TCommand>>();
}