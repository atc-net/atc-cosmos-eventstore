namespace Atc.Cosmos.EventStore.Streams;

internal interface IStreamIndexReader
{
    IAsyncEnumerable<IStreamIndex> ReadAsync(
        string? filter,
        DateTimeOffset? createdAfter,
        CancellationToken cancellationToken);
}