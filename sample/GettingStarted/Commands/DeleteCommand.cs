using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

public record DeleteCommand(string Id, string Reason)
    : CommandBase<SampleEventStreamId>(new SampleEventStreamId(Id));

public class DeleteCommandHandler :
    ICommandHandler<DeleteCommand>,
    IConsumeEvent<AddedEvent>,
    IConsumeEvent<DeletedEvent>
{
    private bool created;
    private bool deleted;

    public void Consume(AddedEvent evt, EventMetadata metadata)
    {
        this.created = true;
    }

    public void Consume(DeletedEvent evt, EventMetadata metadata)
    {
        this.deleted = true;
    }

    public ValueTask ExecuteAsync(
        DeleteCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        if (!created)
        {
            throw new InvalidOperationException("Cannot delete non-existing entity.");
        }

        context.AddEvent(new DeletedEvent(command.Reason));
        return ValueTask.CompletedTask;
    }
}