# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Atc.Cosmos.EventStore** is an event sourcing library for Azure Cosmos DB with built-in CQRS support, published as two NuGet packages:
- `Atc.Cosmos.EventStore` — Core event store (streams, events, subscriptions, checkpoints)
- `Atc.Cosmos.EventStore.Cqrs` — CQRS layer (commands, projections, aggregate state)

Both libraries target **netstandard2.1**. Tests target **net10.0**.

## Build & Test Commands

```bash
# Build entire solution
dotnet build

# Build in release mode (treats warnings as errors)
dotnet build -c release

# Run core event store unit tests
dotnet test test/Atc.Cosmos.EventStore.Tests

# Run CQRS unit tests
dotnet test test/Atc.Cosmos.EventStore.Cqrs.Tests

# Run a single test by fully qualified name
dotnet test test/Atc.Cosmos.EventStore.Tests --filter "FullyQualifiedName~ClassName.MethodName"

# Pack NuGet packages
dotnet pack -c release -o ./packages
```

Integration tests (`Atc.Cosmos.EventStore.IntegrationTests`) require a Cosmos DB instance and are not run in CI.

## Code Style & Conventions

- **C# 11**, nullable reference types enabled, implicit usings enabled
- **File-scoped namespaces** required
- Analyzers enforced: StyleCop, SonarAnalyzer, SecurityCodeScan, .NET Analyzers
- Code style defined in `.editorconfig` (based on [atc-coding-rules](https://github.com/atc-net/atc-coding-rules))
- Test projects use **xunit**, **FluentAssertions**, **NSubstitute** (via Atc.Test), and nullable `annotations` mode (not `enable`)
- Versioning via **Nerdbank.GitVersioning** (`version.json`, base version 1.18)
- Solution uses modern `.slnx` format

### System.IO Removal

All `.csproj` files contain `<Using Remove="System.IO" />`. This removes `System.IO` from implicit usings to avoid ambiguity between `System.IO.Stream` and the library's own stream concepts (`StreamId`, `StreamVersion`, `StreamState`, etc.). If you need `System.IO` types, add an explicit `using System.IO;` in the source file (see `CosmosEventSerializer.cs` for an example).

## Architecture

### Layer Structure

```
Application Code (Commands, Handlers, Projections)
        │
Atc.Cosmos.EventStore.Cqrs
  - Command processing pipeline (ICommandProcessor → ICommandHandler)
  - Projection processing via Change Feed
  - Aggregate state projection (IStateProjector)
        │
Atc.Cosmos.EventStore (Core)
  - IEventStoreClient — main API for read/write/subscribe/query
  - Event streams with optimistic concurrency (StreamVersion + ETag)
  - Event converter pipeline (serialization/deserialization chain)
  - Checkpoint persistence
        │
   ┌────┴────┐
Cosmos DB   In-Memory (testing)
```

### Key Types

- **`StreamId`** — Value type identifying an event stream (implicit string conversion)
- **`StreamVersion`** — Position in a stream. Special values: `StartOfStream` (0), `Any` (max), `NotEmpty` (-1)
- **`EventStreamId`** (CQRS) — Hierarchical dot-separated stream identifier (e.g., `"sample.customer-123"`)
- **`IEventStoreClient`** — Main API: `WriteToStreamAsync`, `ReadFromStreamAsync`, `SubscribeToStreams`, `QueryStreamsAsync`, etc.
- **`IEventCatalog`** — Maps event CLR types ↔ string names for serialization

### CQRS Command Flow

1. `ICommandProcessor<T>.ExecuteAsync(command)` orchestrates the full pipeline
2. `IStateProjector` reads all events from stream, replays them through the handler to build current state
3. `ICommandHandler<T>.ExecuteAsync` runs business logic, adds events to `ICommandContext`
4. `IStateWriter` writes events atomically via Cosmos DB `TransactionalBatch`
5. On conflict: retries based on `ICommand.Behavior` (`OnConflict.Retry` or `OnConflict.RerunCommand`)
6. Returns `CommandResult` with `ResultType`: Changed, Conflict, Exists, NotFound, NotModified

### Cosmos DB Storage

- Events stored as JSON documents partitioned by stream ID (`/pk`)
- Metadata document co-located in same partition (ETag-based concurrency)
- Three containers: events, subscriptions (change feed leases), stream index
- `CosmosEventStoreInitializer` creates database/containers with proper indexing

### Projections

- Implement `IProjection` + `IConsumeEvent<T>` / `IConsumeEventAsync<T>`
- Filtered by stream ID pattern via `ProjectionFilterAttribute` (LIKE matching, e.g., `"sample.*"`)
- Driven by Cosmos DB Change Feed Processor
- Registered via `builder.AddProjectionJob<T>()`

### DI Registration

```csharp
services.AddEventStore(builder =>
{
    builder.UseCosmosDb();                                    // or UseInMemoryDb()
    builder.UseEvents(c => c.FromAssembly<SomeEvent>());     // register event catalog
    builder.UseCQRS(c =>
    {
        c.AddCommandsFromAssembly<SomeCommand>();
        c.AddProjectionJob<SomeProjection>();
    });
});
```

### Event Converter Pipeline

Deserialization uses a chain-of-responsibility pattern through `IEventDataConverter` implementations:
- `NamedEventConverter` — resolves type from EventCatalog
- `EventDocumentConverter` — JSON deserialization
- `UnknownEventDataConverter` — wraps unregistered event types as `UnknownEvent`
- `FaultedEventDataConverter` — wraps deserialization failures as `FaultedEvent`

## Solution Structure

```
src/
  Atc.Cosmos.EventStore/           # Core library
  Atc.Cosmos.EventStore.Cqrs/      # CQRS extension
test/
  Atc.Cosmos.EventStore.Tests/     # Core unit tests
  Atc.Cosmos.EventStore.Cqrs.Tests/ # CQRS unit tests (includes functional tests with TestHost)
  Atc.Cosmos.EventStore.IntegrationTests/  # Requires Cosmos DB
sample/
  GettingStarted/                  # Sample console app
```

## CI/CD

- **verification.yml** — Builds, tests, and packs on PRs and pushes to main
- **release-preview.yml** — Publishes preview packages after verification passes on main
- **prepare-release.yml** — Creates release branch and PR to stable (manual trigger)
- **release.yml** — Publishes to NuGet.org on merge to stable, creates GitHub release
