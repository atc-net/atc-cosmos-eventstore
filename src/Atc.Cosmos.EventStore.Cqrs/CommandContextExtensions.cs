namespace Atc.Cosmos.EventStore.Cqrs;

public static class CommandContextExtensions
{
    public static ICommandContext AddEventWhen(
        this ICommandContext context,
        Func<bool> condition,
        Func<object> eventProvider)
        => condition()
         ? context.OnAddEvent(eventProvider())
         : context;

    public static ValueTask AsAsync(
        this ICommandContext context)
        => default; // default is a completed value task.

    private static ICommandContext OnAddEvent(
        this ICommandContext context,
        object @event)
    {
        context.AddEvent(@event);

        return context;
    }
}
