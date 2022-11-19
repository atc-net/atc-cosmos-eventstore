namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamIterator
{
    IAsyncEnumerable<IEvent> ReadAsync(
        StreamId streamId,
        StreamVersion fromVersion,
        StreamReadFilter? filter,
        CancellationToken cancellationToken);
}