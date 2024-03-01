using Atc.Cosmos.EventStore.Cqrs;

namespace GettingStarted;

public record CreateCommand(string Id, string Name)
    : CommandBase<FooEventStreamId>(new FooEventStreamId(Id));

public class CreateCommandHandler :
    ICommandHandler<CreateCommand>,
    IConsumeEvent<CreateCommand>
{
    private bool created;

    public void Consume(CreateCommand evt, EventMetadata metadata)
    {
        this.created = true;
    }

    public ValueTask ExecuteAsync(
        CreateCommand command,
        ICommandContext context,
        CancellationToken cancellationToken)
    {
        if (!created)
        {
            context.AddEvent(new AddedEvent(command.Name));
        }

        return ValueTask.CompletedTask;
    }
}