namespace Atc.Cosmos.EventStore.Cqrs
{
    public interface ICommand
    {
        /// <summary>
        /// Gets the unique id of command instance.
        /// </summary>
        string CommandId { get; }

        /// <summary>
        /// Gets correlation id used to track a request through various systems and services.
        /// </summary>
        string? CorrelationId { get; }

        /// <summary>
        /// Gets the required version, when executing the command.
        /// </summary>
        EventStreamVersion? RequiredVersion { get; }

        /// <summary>
        /// Gets the behavior when stream conflict occurs.
        /// </summary>
        OnConflict Behavior { get; }

        /// <summary>
        /// Gets the number of times to rerun or retry the command when receiving a conflict.
        /// </summary>
        int BehaviorCount { get; }

        /// <summary>
        /// Gets the id of event stream.
        /// </summary>
        /// <returns>Event stream id.</returns>
        EventStreamId GetEventStreamId();
    }
}
