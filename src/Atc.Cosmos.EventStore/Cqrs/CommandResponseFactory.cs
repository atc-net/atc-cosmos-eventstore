namespace BigBang.Cosmos.EventStore.Cqrs
{
    public static class CommandResponseFactory
    {
        /// <summary>
        /// Gets a <seealso cref="CommandResponse"/> from an <seealso cref="EventStreamResponse"/>.
        /// </summary>
        /// <param name="response">Response to get the command response from.</param>
        /// <returns>Command response.</returns>
        public static CommandResponse FromEventStreamResponse(EventStreamResponse response)
            => response switch
            {
                EventStreamResponse m when m.IsSuccess => new SuccessfulCommandResponse(m.StreamId, m.Etag),
                EventStreamResponse m when m.HasConflict => new ConflictCommandResponse(m.StreamId, m.Etag, m.ErrorMessage),
                EventStreamResponse m => new FailedCommandResponse(m.StreamId, m.ErrorMessage, m.StatusCode),
            };
    }
}