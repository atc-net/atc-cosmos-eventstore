using System;
using BigBang.Cosmos.EventStore.Cqrs;

namespace BigBang.Cosmos.EventStore
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IEventStoreClient client;

        public CommandFactory(IEventStoreClient client)
        {
            this.client = client;
        }

        public ICommandDefinition DefineCommand(string commandName, string streamName, Action<CommandOptions> configure)
        {
            var stream = client.GetVersionedStream(streamName);
            var options = new CommandOptions(commandName);
            configure?.Invoke(options);

            return new CommandDefinition(stream, options);
        }
    }
}