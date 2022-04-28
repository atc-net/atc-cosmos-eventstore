namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface ICommandContext
    {
        /// <summary>
        /// Adds an event object to the stream.
        /// </summary>
        /// <remarks>
        /// Events added are not committed to stream until after
        /// <seealso cref="ICommandHandler{TCommand}.ExecuteAsync(TCommand, ICommandContext, System.Threading.CancellationToken)"/>
        /// has completed.
        /// </remarks>
        /// <param name="evt">Object to add.</param>
        void AddEvent(object evt);

        /// <summary>
        /// Gets or sets an optional response object from the command.
        /// </summary>
        /// <remarks>
        /// When value is set it can be accessed from <see cref="CommandResult.Response"/>
        /// received when calling <see cref="ICommandProcessor{TCommand}.ExecuteAsync(TCommand, System.Threading.CancellationToken)"/>.
        /// </remarks>
        object? ResponseObject { get; set; }

        /// <summary>
        /// Gets the current of the stream such as position and id.
        /// </summary>
        IEventStreamState StreamState { get; }
    }
}