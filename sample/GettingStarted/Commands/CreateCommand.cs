namespace GettingStarted.Commands;

public record CreateCommand(string Id, string Name, string Address)
    : CommandBase<SampleEventStreamId>(new SampleEventStreamId(Id));