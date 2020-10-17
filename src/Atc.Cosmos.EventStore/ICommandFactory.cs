using System;
using BigBang.Cosmos.EventStore.Cqrs;

namespace BigBang.Cosmos.EventStore
{
    public interface ICommandFactory
    {
        ICommandDefinition DefineCommand(string commandName, string streamName, Action<CommandOptions> configure);
    }
}