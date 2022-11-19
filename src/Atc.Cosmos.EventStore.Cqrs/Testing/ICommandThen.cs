namespace Atc.Cosmos.EventStore.Cqrs.Testing;

public interface ICommandThen
{
    Task ThenExpectEvents(
        Action<ICommandContextInspector> assert,
        CancellationToken cancellationToken = default);

    Task ThenExpectException<TException>(
        CancellationToken cancellationToken = default)
        where TException : Exception;

    Task ThenExpectException<TException>(
        Predicate<TException> predicate,
        CancellationToken cancellationToken = default)
        where TException : Exception;
}