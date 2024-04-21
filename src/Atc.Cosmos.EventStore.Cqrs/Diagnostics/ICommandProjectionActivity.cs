namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public interface ICommandProjectionActivity
    : IDisposable
{
    void Completed(StreamVersion version);
}