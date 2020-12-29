using System.Net;

namespace BigBang.Cosmos.EventStore.Cqrs
{
    public class FailedCommandResponse : CommandResponse
    {
        public FailedCommandResponse(string streamId, string message, HttpStatusCode statusCode)
            : base(streamId, false)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public HttpStatusCode StatusCode { get; }

        public string Message { get; }
    }
}