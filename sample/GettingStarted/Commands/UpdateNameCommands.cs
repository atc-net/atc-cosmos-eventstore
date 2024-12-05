using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

public record UpdateNameCommand(string Id, string Name)
    : CommandBase<SampleEventStreamId>(new SampleEventStreamId(Id));

public class UpdateNameCommandHandler :
    ICommandHandler<UpdateNameCommand>,
    IConsumeEvent<AddedEvent>,
    IConsumeEvent<NameChangedEvent>
{
    private bool created;
    private string? currentName;

    public void Consume(AddedEvent evt, EventMetadata metadata)
    {
        created = true;
        currentName = evt.Name;
    }

    public void Consume(NameChangedEvent evt, EventMetadata metadata)
    {
        currentName = evt.NewName;
    }

    public ValueTask ExecuteAsync(
        UpdateNameCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        if (!created)
        {
            throw new InvalidOperationException("Cannot change name on non-existing entity.");
        }

        if (currentName != command.Name)
        {
            context.AddEvent(new NameChangedEvent(currentName!, command.Name));
        }

        return ValueTask.CompletedTask;
    }
}