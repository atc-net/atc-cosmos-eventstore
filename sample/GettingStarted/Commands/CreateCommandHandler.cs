namespace GettingStarted.Commands;

public class CreateCommandHandler :
    ICommandHandler<CreateCommand>,
    IConsumeEvent<CreateCommand>
{
    private bool created;

    public void Consume(
        CreateCommand evt,
        EventMetadata metadata)
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
            context.AddEvent(new AddedEvent(command.Name, command.Address));
        }

        return ValueTask.CompletedTask;
    }
}