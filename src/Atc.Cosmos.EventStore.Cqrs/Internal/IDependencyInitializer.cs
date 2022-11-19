namespace Atc.Cosmos.EventStore.Cqrs.Internal;

internal interface IDependencyInitializer
{
    Task EnsureInitializeAsync();
}