using System;
using Microsoft.Extensions.DependencyInjection;

namespace Atc.Cosmos.EventStore.Cqrs.Commands
{
    public class CommandProcessorFactory : ICommandProcessorFactory
    {
        private readonly IServiceProvider provider;

        public CommandProcessorFactory(
            IServiceProvider provider)
        {
            this.provider = provider;
        }

        public ICommandProcessor<TCommand> Create<TCommand>()
            where TCommand : ICommand
            => provider.GetRequiredService<ICommandProcessor<TCommand>>();
    }
}