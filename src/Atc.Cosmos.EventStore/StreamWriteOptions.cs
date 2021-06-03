namespace Atc.Cosmos.EventStore
{
    public class StreamWriteOptions
    {
        /// <summary>
        /// Gets or sets correlation id used to track a request through various systems and services.
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the id of the event or command that caused the events to be written.
        /// </summary>
        public string? CausationId { get; set; }
    }
}