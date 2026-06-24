namespace GettingStarted.Commands;

public record DeleteCommand(string Id, string Reason)
    : CommandBase<SampleEventStreamId>(new SampleEventStreamId(Id));