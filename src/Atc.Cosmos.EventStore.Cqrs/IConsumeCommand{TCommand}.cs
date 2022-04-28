using System.Threading;
using System.Threading.Tasks;

namespace Atc.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Instructs the CQRS runtime to invoke the <seealso cref="ConsumeCommandAsync(TCommand, IStreamMetadata, CancellationToken)"/>
    /// before starting to consume events from the stream into the command handler.
    /// </summary>
    /// <typeparam name="TCommand">Type of command.</typeparam>
    public interface IConsumeCommand<in TCommand>
        where TCommand : ICommand
    {
        ValueTask ConsumeCommandAsync(
            TCommand command,
            IStreamMetadata metadata,
            CancellationToken cancellationToken);
    }

    public interface ICheckpointManager
    {
        IStreamMetadata Metadata { get; }

        Task<Checkpoint<TState>?> GetCheckpointAsync<TState>(
            string name,
            CancellationToken cancellationToken);

        Task<TState?> ResumeFromCheckpointAsync<TState>(
            string name,
            CancellationToken cancellationToken);
    }
}