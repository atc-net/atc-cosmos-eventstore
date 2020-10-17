namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class ConflictCommandResponse : CommandResponse
    {
        public ConflictCommandResponse(string streamId, string etag, string message)
            : base(streamId, false)
        {
            Etag = etag;
            Message = message;
        }

        public string Etag { get; }
        public string Message { get; }
    }
}