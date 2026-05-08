namespace GettingStarted.Commands;

public record UpdateNameCommand(string Id, string Name)
    : CommandBase<SampleEventStreamId>(new SampleEventStreamId(Id));