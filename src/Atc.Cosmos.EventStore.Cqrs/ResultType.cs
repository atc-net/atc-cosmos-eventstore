namespace Atc.Cosmos.EventStore.Cqrs
{
    public enum ResultType
    {
        /// <summary>
        /// Command resulted in successfully writing one or more events to stream.
        /// </summary>
        Changed,

        /// <summary>
        /// Command yielded no events to write to stream.
        /// </summary>
        NotModified,

        /// <summary>
        /// Current stream version was not at the required position.
        /// </summary>
        Conflict,

        /// <summary>
        /// Command failed as it required the stream to container one or more events, but an empty stream was found.
        /// </summary>
        NotFound,

        /// <summary>
        /// Command failed as it required the stream to be empty, but one or more events was found.
        /// </summary>
        Exists,
    }
}