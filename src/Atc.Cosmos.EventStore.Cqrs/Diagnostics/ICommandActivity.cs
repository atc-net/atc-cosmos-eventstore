namespace Atc.Cosmos.EventStore.Cqrs.Diagnostics;

public interface ICommandActivity : IDisposable
{
    void Changed();

    void Conflict();

    void NotModified();
}
