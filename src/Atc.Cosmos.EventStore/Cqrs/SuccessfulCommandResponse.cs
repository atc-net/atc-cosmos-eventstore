namespace BigBang.Cosmos.EventStore.Cqrs
{
    /// <summary>
    /// Represents a successfully process command.
    /// </summary>
    public class SuccessfulCommandResponse : CommandResponse
    {
        public SuccessfulCommandResponse(string streamId, string etag)
            : base(streamId, true)
        {
            Etag = etag;
        }

        public string Etag { get; }
    }
}