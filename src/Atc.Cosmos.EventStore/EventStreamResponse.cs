using System.Net;

namespace BigBang.Cosmos.EventStore
{
    public class EventStreamResponse
    {
        public EventStreamResponse(string message, HttpStatusCode statusCode, string streamId, string etag)
        {
            ErrorMessage = message;
            StatusCode = statusCode;
            StreamId = streamId;
            Etag = etag;
        }

        public EventStreamResponse(string message, HttpStatusCode statusCode)
            : this (message, statusCode, string.Empty, string.Empty)
        { }

        public EventStreamResponse(string streamId, string etag)
            : this (string.Empty, HttpStatusCode.OK, streamId, etag)
        { }

        public EventStreamResponse(string streamId, string etag, HttpStatusCode statusCode)
            : this(string.Empty, statusCode, streamId, etag)
        { }

        public EventStreamResponse()
            : this (string.Empty, HttpStatusCode.OK)
        { }

        public string StreamId { get; }

        public string Etag { get; }

        public HttpStatusCode StatusCode { get; }

        public string ErrorMessage { get; }

        public bool HasConflict
            => StatusCode == HttpStatusCode.Conflict;

        public bool IsSuccess
            => StatusCode == HttpStatusCode.OK
            || StatusCode == HttpStatusCode.Created
            || StatusCode == HttpStatusCode.NotModified;
    }
}