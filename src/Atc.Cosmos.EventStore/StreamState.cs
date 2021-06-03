namespace Atc.Cosmos.EventStore
{
    public enum StreamState
    {
        /// <summary>
        /// Stream is new and contains zero events.
        /// </summary>
        New,

        /// <summary>
        /// Stream is active and contains one or more events.
        /// </summary>
        Active,

        /// <summary>
        /// Stream is not longer accepting events as it has been closed.
        /// </summary>
        Closed,
    }
}