namespace BigBang.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Responsible for resolving event store write conflicts.
    /// </summary>
    public interface IConflictResolver
    {
        /// <summary>
        /// Determines if a event store write operation must be retries based on its response.
        /// </summary>
        /// <param name="response">Command response from the write operation.</param>
        /// <param name="etag">Etag of the write operation.</param>
        /// <returns>True if the operation should be retries, otherwise false.</returns>
        bool RetryRequired(CommandResponse response, string? etag);
    }
}