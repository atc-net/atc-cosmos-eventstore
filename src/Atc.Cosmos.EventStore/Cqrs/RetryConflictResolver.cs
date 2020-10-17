namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class RetryConflictResolver : IConflictResolver
    {
        private byte maxRetries;

        public RetryConflictResolver(byte maxRetries)
        {
            this.maxRetries = maxRetries;
        }

        public bool RetryRequired(CommandResponse response, string? etag)
            => response switch
            {
                // If an etag was provided then skip the retry as it will always fail.
                ConflictCommandResponse _ when etag != null => false,
                ConflictCommandResponse _ => maxRetries-- > 0,
                _ => false,
            };
    }
}