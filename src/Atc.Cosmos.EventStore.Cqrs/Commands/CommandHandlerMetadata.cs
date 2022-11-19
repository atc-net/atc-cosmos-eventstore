using Atc.Cosmos.EventStore.Cqrs.Internal;

namespace Atc.Cosmos.EventStore.Cqrs.Commands;

internal class CommandHandlerMetadata<TCommand, THandler> :
    ConsumeEventMetadata,
    ICommandHandlerMetadata<TCommand>
    where TCommand : ICommand
    where THandler : ICommandHandler<TCommand>
{
    public CommandHandlerMetadata()
        : base(typeof(THandler))
    {
    }

    public ValueTask ConsumeAsync(
        IEvent evt,
        ICommandHandler<TCommand> handler,
        CancellationToken cancellationToken)
        => base.ConsumeAsync(evt, handler, cancellationToken);
}
