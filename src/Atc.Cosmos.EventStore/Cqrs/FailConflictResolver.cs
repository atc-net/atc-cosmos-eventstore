namespace BigBang.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// test.
    /// </summary>
    public class FailConflictResolver : IConflictResolver
    {
        public bool RetryRequired(CommandResponse response, string? etag)
            => false;
    }
}