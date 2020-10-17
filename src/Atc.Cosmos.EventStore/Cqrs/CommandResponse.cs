namespace BigBang.Cosmos.EventStore.Cqrs
{
    public abstract class CommandResponse
    {
        protected CommandResponse(string streamId, bool isSuccess)
        {
            StreamId = streamId;
            IsSuccess = isSuccess;
        }

        public string StreamId { get; }

        /// <summary>
        /// Gets a value indicating whether the command was processed.
        /// </summary>
        public bool IsSuccess { get; }
    }
}