[![NuGet Version](https://img.shields.io/nuget/v/atc.cosmos.eventstore.svg?logo=nuget&style=for-the-badge&label=Atc.Cosmos.EventStore)](https://www.nuget.org/packages/atc.cosmos.eventstore)
[![NuGet Version](https://img.shields.io/nuget/v/atc.cosmos.eventstore.cqrs.svg?logo=nuget&style=for-the-badge&label=Atc.Cosmos.EventStore.Cqrs)](https://www.nuget.org/packages/atc.cosmos.eventstore.cqrs)

# Introduction

Atc.Cosmos.EventStore is a .NET library that provides event sourcing on top of Azure Cosmos DB. The companion Atc.Cosmos.EventStore.Cqrs package layers a CQRS model on top, with commands, handlers, aggregate state replay, and projections driven by the Cosmos DB change feed.

The design leverages Cosmos DB's natural strengths — partitioned containers, ETag-based concurrency, transactional batches, and the change feed — to provide a streamlined developer experience for building event-sourced services.

## Table of Contents

- [Introduction](#introduction)
  - [Table of Contents](#table-of-contents)
  - [Features](#features)
  - [Packages](#packages)
  - [Core Concepts](#core-concepts)
    - [Event Sourcing in One Paragraph](#event-sourcing-in-one-paragraph)
    - [CQRS in One Paragraph](#cqrs-in-one-paragraph)
    - [Glossary](#glossary)
    - [How a Command Flows Through the System](#how-a-command-flows-through-the-system)
  - [Getting Started](#getting-started)
    - [Installation](#installation)
    - [Configuring with ServiceCollection Extensions](#configuring-with-servicecollection-extensions)
    - [Cosmos DB Containers](#cosmos-db-containers)
  - [Events](#events)
    - [Defining an Event](#defining-an-event)
    - [How Events Are Persisted](#how-events-are-persisted)
    - [Versioning](#versioning)
  - [Event Streams](#event-streams)
    - [What a Stream Is and Why It Exists](#what-a-stream-is-and-why-it-exists)
    - [StreamId as Partition Key](#streamid-as-partition-key)
    - [Strongly-Typed Stream Identifiers](#strongly-typed-stream-identifiers)
    - [Stream Versions](#stream-versions)
    - [Stream Metadata](#stream-metadata)
  - [Commands](#commands)
    - [What a Command Is](#what-a-command-is)
    - [Defining a Command](#defining-a-command)
    - [Command Handlers](#command-handlers)
    - [State Replay](#state-replay)
    - [Asynchronous Event Consumption](#asynchronous-event-consumption)
    - [Executing Commands](#executing-commands)
    - [Optimistic Concurrency and Conflict Resolution](#optimistic-concurrency-and-conflict-resolution)
  - [Projections](#projections)
    - [What a Projection Is](#what-a-projection-is)
    - [Defining a Projection](#defining-a-projection)
    - [How Projections Run](#how-projections-run)
    - [Projection Filters](#projection-filters)
    - [Why Projections Cannot Write Events](#why-projections-cannot-write-events)
    - [Read-Model Storage](#read-model-storage)
    - [Registering Projections](#registering-projections)
    - [Starting From a Specific Date](#starting-from-a-specific-date)
  - [In-Memory Event Store](#in-memory-event-store)
  - [Sample](#sample)
- [Requirements](#requirements)
- [How to contribute](#how-to-contribute)

## Features

- ✅ **Event-sourced streams**: Streams partitioned by id with append-only semantics.
- ✅ **Optimistic concurrency**: Conflict detection via ETag on the stream metadata document.
- ✅ **Atomic writes**: Events and metadata persisted in a single Cosmos `TransactionalBatch`.
- ✅ **Strongly-typed event catalogue**: `[StreamEvent]` attributes with built-in versioning.
- ✅ **CQRS pipeline**: Commands, handlers, aggregate state replay, and conflict retries.
- ✅ **Change-feed projections**: Read models built from the Cosmos DB change feed with persistent leases.
- ✅ **Projection filters**: Wildcard-based stream filtering so projections only see relevant events.
- ✅ **In-memory implementation**: A drop-in event store for unit and integration tests.

## Packages

| Package                            | Purpose                                                        |
| ---------------------------------- | -------------------------------------------------------------- |
| `Atc.Cosmos.EventStore`            | Core event store: streams, events, subscriptions, checkpoints. |
| `Atc.Cosmos.EventStore.Cqrs`       | CQRS layer: commands, handlers, aggregate state, projections.  |

Both packages target `netstandard2.1`.

## Core Concepts

If you have not worked with event sourcing or CQRS before, the next few sections give you just enough vocabulary to follow the rest of the document. The library is opinionated about these patterns, and the API shapes follow directly from them.

### Event Sourcing in One Paragraph

In a traditional application, the database stores the **current state** — a `Customer` row with `Name = "Alice"`. In an event-sourced application, the database stores the **sequence of facts** that led to that state — `CustomerCreated`, `NameChanged("Alice")`. The current state is *derived* by replaying those facts in order. The events are the source of truth; any other view of the data (a row in a table, a document in a search index) is just a projection of the events. This gives you a complete audit trail, the ability to rebuild state at any point in history, and the freedom to add new read-side views later by replaying old events.

### CQRS in One Paragraph

CQRS — Command Query Responsibility Segregation — splits the model used for **writing** from the model used for **reading**. Writes go through *commands* that express intent (`CreateCustomer`, `ChangeName`); each command is validated and turned into one or more events that are appended to a stream. Reads go through *projections* that listen for new events and update purpose-built **read models** optimised for the queries the application makes (a customer document, a search index, a denormalised report). The two sides are eventually consistent: a successful command does not block on the read side catching up. CQRS pairs naturally with event sourcing because the events that come out of the write side are exactly what the read side needs to consume.

### Glossary

| Term                    | Description                                                                              | Library type                                          |
| ----------------------- | ---------------------------------------------------------------------------------------- | ----------------------------------------------------- |
| **Event**               | An immutable fact describing something that happened.                                    | Record decorated with `[StreamEvent("name:v1")]`      |
| **Stream**              | The ordered sequence of events for a single aggregate. Lives in its own Cosmos partition. | Identified by `EventStreamId`                         |
| **Aggregate**           | The conceptual entity whose state is derived from its stream (no class — it's rebuilt in memory). | Reconstructed by handler state replay                 |
| **Command**             | A request to change state. May succeed, fail, or be a no-op.                             | `CommandBase<TStreamId>`                              |
| **Command Handler**     | Validates the command, applies business rules, and emits events.                         | `ICommandHandler<TCommand>`                           |
| **State Replay**        | Rebuilding current state by feeding the stream's existing events into the handler.       | `IConsumeEvent<TEvent>` on the handler                |
| **Optimistic Concurrency** | Two writers may race; the loser sees a conflict and retries.                          | Stream metadata ETag + `OnConflict` retry policies    |
| **Projection**          | A worker that consumes events from streams and updates a read model or triggers side effects. | `IProjection` + `IConsumeEvent<TEvent>`               |
| **Read Model**          | A view of the data optimised for queries. Eventually consistent with the events.         | Any storage you choose (Cosmos, SQL, search, …)       |
| **Change Feed**         | Cosmos DB's ordered, durable stream of changes per partition. Drives projection delivery. | Cosmos DB Change Feed Processor (provided)            |

### How a Command Flows Through the System

```
   ┌─────────────┐
   │   Client    │
   └──────┬──────┘
          │ command
          ▼
   ┌──────────────────────────────────────────────┐
   │ ICommandProcessor<TCommand>                  │
   │  1. Read existing events from the stream     │
   │  2. Replay them into the handler             │  ◀── state rebuilt in memory
   │  3. Call handler.ExecuteAsync                │
   │  4. Write new events as a single batch       │  ◀── atomic, ETag-checked
   │  5. Retry on conflict (per OnConflict policy)│
   └──────────────────────────────────────────────┘
          │ events
          ▼
   ┌──────────────────────────────────────────────┐
   │ event-store container (source of truth)      │
   └──────────────────────────────────────────────┘
          │ Cosmos DB Change Feed
          ▼
   ┌──────────────────────────────────────────────┐
   │ Projection                                   │
   │   InitializeAsync → Consume → CompleteAsync  │
   └──────────────────────────────────────────────┘
          │
          ▼
   Read Model (Cosmos / SQL / search / …)
          │
          ▼
   ┌─────────────┐
   │   Client    │  queries the read model directly
   └─────────────┘
```

The two paths — command/write on the way down, projection/read on the way back up — never cross synchronously. That separation is what makes the system scale and what makes the audit trail unconditional.

## Getting Started

### Installation

```bash
dotnet add package Atc.Cosmos.EventStore
dotnet add package Atc.Cosmos.EventStore.Cqrs
```

### Configuring with ServiceCollection Extensions

Configure the event store and the CQRS layer through the standard dependency injection container. `AddEventStore` registers the core services, and the `UseCQRS` callback enables the command and projection pipeline:

```csharp
services.ConfigureOptions<ConfigureEventStoreOptions>();
services.AddEventStore(builder =>
{
    builder.UseCosmosDb();
    builder.UseEvents(c => c.FromAssembly<AddedEvent>());
    builder.UseCQRS(c =>
    {
        c.AddInitialization(
            throughput: 4000,
            serviceProvider => serviceProvider
                .GetRequiredService<ICosmosInitializer>()
                .InitializeAsync(CancellationToken.None));

        c.AddCommandsFromAssembly<CreateCommand>();
        c.AddProjectionJob<SampleProjection>(nameof(SampleProjection));
    });
});
```

`ConfigureEventStoreOptions` is an `IConfigureOptions<EventStoreClientOptions>` implementation that supplies the database name, connection mode, and any other client settings. For local development, replace `UseCosmosDb()` with `UseInMemoryDb()`.

### Cosmos DB Containers

When the initializer runs, the following containers are created in the configured database:

| Container         | Purpose                                                          |
| ----------------- | ---------------------------------------------------------------- |
| `event-store`     | Persists events and per-stream metadata documents.               |
| `stream-index`    | Stores the searchable stream index used by `QueryStreamsAsync`.  |
| `subscriptions`   | Holds the change-feed leases used by projection subscriptions.   |

## Events

### Defining an Event

Events are immutable records decorated with the `[StreamEvent]` attribute. The attribute name is what gets persisted to Cosmos DB as a string discriminator, and includes a version segment so future revisions can coexist with historical data:

```csharp
[StreamEvent("added-event:v1")]
public record AddedEvent(string Name, string Address);
```

Events are auto-discovered from an assembly via `UseEvents(c => c.FromAssembly<TMarker>())`. The library scans the assembly, builds an `IEventCatalog` mapping each `[StreamEvent]` name to its CLR type, and uses that catalogue both when serialising new events and when deserialising existing ones during replay or projection.

### How Events Are Persisted

Each event becomes a JSON document in the `event-store` container. The document carries:

- The serialised event payload (the record's properties).
- A `properties.name` field set to the `[StreamEvent]` discriminator (e.g. `"added-event:v1"`).
- A `properties.version` field set to the event's position in its stream (1, 2, 3, …).
- A `pk` field set to the stream id, so the document lives in the partition for that stream.

When the library reads events back, the deserialisation goes through a chain-of-responsibility pipeline:

1. `NamedEventConverter` resolves the CLR type from the `name` field via the catalogue.
2. `EventDocumentConverter` deserialises the payload into the resolved type.
3. `UnknownEventDataConverter` wraps events whose name is not in the catalogue as `UnknownEvent` so handlers and projections see them as opaque rather than crashing.
4. `FaultedEventDataConverter` wraps events that fail deserialisation as `FaultedEvent` for the same reason.

This is why an unknown event in a stream does not break a projection — it just shows up as `UnknownEvent` and the projection can ignore it or log it.

### Versioning

Events that have been written to a stream live forever. The library treats `[StreamEvent]` records as append-only contracts:

- Existing event records are never deleted or mutated in incompatible ways.
- When the shape of an event needs to change, a new record is added with a bumped version (`added-event:v2`).
- The previous version is marked `[Obsolete]` so consumers know to migrate, but it remains in the catalogue so historical events still deserialise.

A handler that needs to react to both versions implements `IConsumeEvent<AddedEventV1>` *and* `IConsumeEvent<AddedEventV2>` — typically the V1 handler upcasts to the V2 shape so the rest of the logic stays uniform.

## Event Streams

### What a Stream Is and Why It Exists

A **stream** is the unit of consistency in the event store. All events for one logical entity — one customer, one order, one device — go into a single stream, and the stream is the boundary for ordering, atomicity, and concurrency control.

Streams matter because the library needs a way to answer three questions for every write:

1. **Where do these events go?** — into the named stream.
2. **What state were we in when this command ran?** — the events that were already in that stream.
3. **Did anyone else write to the stream while we were thinking?** — a fast equality check on the stream's current version/ETag.

A stream id is the answer to question 1, and the rest of the library is built around it.

### StreamId as Partition Key

`StreamId` is a value type that implicitly converts to and from `string`. Its value is used directly as the Cosmos DB partition key (`/pk`) on the `event-store` container. This has practical consequences:

- All events for one stream live in the same logical partition, which is what makes ordered reads, ETag-based concurrency, and `TransactionalBatch` writes possible.
- The library does **not** support transactions that span multiple streams. If two aggregates need to change atomically, they belong in the same stream — or you accept eventual consistency between them.
- Stream-level write throughput is bounded by what one logical partition can sustain. Hot-spotting a single stream is the most common scaling pitfall.

### Strongly-Typed Stream Identifiers

`EventStreamId` (CQRS layer) wraps a `StreamId` as a hierarchical, dot-separated identifier. The hierarchy exists for two reasons:

- **Type prefixing** — `sample.123`, `customer.abc`, `order.42` keeps different aggregate types in distinct id namespaces.
- **Wildcard filtering** — projections can subscribe to `sample.*` or `sample.**` rather than every stream in the database (see [Projection Filters](#projection-filters)).

Applications typically derive a strongly-typed identifier per aggregate so the rest of the code never works with raw strings:

```csharp
public sealed class SampleEventStreamId : EventStreamId, IEquatable<SampleEventStreamId?>
{
    public const string FilterIncludeAllEvents = TypeName + ".*";

    private const string TypeName = "sample";

    public SampleEventStreamId(string id)
        : base(TypeName, id)
    {
        Id = id;
    }

    public SampleEventStreamId(EventStreamId id)
        : base(id.Parts.ToArray())
    {
        Id = id.Parts[1];
    }

    public string Id { get; }

    public override bool Equals(object? obj)
        => Equals(obj as SampleEventStreamId);

    public bool Equals(SampleEventStreamId? other)
        => other != null && Value == other.Value;

    public override int GetHashCode()
        => HashCode.Combine(Value);
}
```

The `FilterIncludeAllEvents` constant doubles as a projection filter so all streams of this type are routed to the same read model. Defining it next to the id keeps the naming consistent across writers and readers.

### Stream Versions

`StreamVersion` is the position of an event in its stream — the first event written is version 1, the next is version 2, and so on. Every command implicitly carries a `RequiredVersion` that the library checks against the stream before writing:

| Special value     | Meaning                                                                  |
| ----------------- | ------------------------------------------------------------------------ |
| `StartOfStream`   | The stream must be empty (version 0). Used to enforce "create only".     |
| `Any`             | Accept whatever version the stream is currently at. The default.         |
| `NotEmpty`        | The stream must already have at least one event (version ≥ 1).           |

When a handler emits new events, they are assigned the next sequential versions, written in a single batch, and the stream's metadata document is bumped to the new highest version. If two writers race for the same `RequiredVersion`, only one of them wins.

### Stream Metadata

Alongside the events, each stream has a single **metadata document** in the same partition. This document holds the stream's current version, timestamps, and any user-supplied metadata, and its Cosmos `_etag` is what the library uses to detect concurrent writes:

- A `TransactionalBatch` updates the metadata document and inserts the new events as a single atomic operation.
- The batch is conditioned on the metadata's current `_etag` matching the value that was read at the start of command processing.
- If another writer committed first, the ETag has changed, the batch is rejected, and the command pipeline moves into the conflict-resolution path described below.

This is the mechanism behind the "atomic, ETag-checked" step in the lifecycle diagram above.

## Commands

### What a Command Is

A command is a *request to change state*. It is not a method call into the domain — it's a serialisable record describing intent (`CreateCustomer`, `ChangeName`) that the library hands to a dedicated handler. The handler decides whether the request is valid, what events should be appended to the stream, and what to return to the caller. Because a command is data, it can be queued, audit-logged, replayed in tests, or dispatched from a UI without the rest of the domain leaking out.

Each command carries the id of the stream it targets — that is how the pipeline knows where to read existing events from and where to write new ones to. `CommandBase<TStreamId>` is the base record that wires this up.

### Defining a Command

Commands are records that extend `CommandBase<TStreamId>` and carry the data needed to process the operation:

```csharp
public record CreateCommand(string Id, string Name, string Address)
    : CommandBase<SampleEventStreamId>(new SampleEventStreamId(Id));
```

The base class implements `ICommand` and exposes properties the pipeline reads when processing the command: `EventStreamId` (computed from `TStreamId`), `RequiredVersion` (defaults to `Any`), `Behavior` (defaults to `OnConflict.Fail`), `BehaviorCount` (the retry cap when a retry mode is chosen), `CommandId`, and `CorrelationId`. Override any of them in the derived record when you need different behaviour — for example, a "create only" command sets `RequiredVersion` to `StartOfStream`, and an idempotent command typically sets `Behavior = OnConflict.RerunCommand`.

### Command Handlers

A handler implements `ICommandHandler<TCommand>` and uses `ICommandContext` to record events:

```csharp
public class CreateCommandHandler :
    ICommandHandler<CreateCommand>,
    IConsumeEvent<AddedEvent>
{
    private bool created;

    public void Consume(AddedEvent evt, EventMetadata metadata)
        => created = true;

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
```

`context.AddEvent` queues events; the framework writes them in a single Cosmos `TransactionalBatch` after `ExecuteAsync` returns, preserving atomicity per stream. `context.ResponseObject` lets the handler return a payload to the caller. Handlers are resolved per-command from the DI container, so each invocation gets a fresh instance and any captured state (`created` above) starts clean.

### State Replay

Handlers are *stateless looking* by design — but the pipeline gives them a chance to learn the current state of the aggregate before `ExecuteAsync` runs. The mechanism is `IConsumeEvent<TEvent>` (or `IConsumeEventAsync<TEvent>`).

When a command arrives, the `ICommandProcessor<TCommand>` does this, in order:

1. Resolves a fresh `ICommandHandler<TCommand>` from DI.
2. Reads every event from the target stream.
3. For each event, looks for a `Consume(TEvent, EventMetadata)` overload on the handler that matches the event's CLR type and calls it.
4. Calls `handler.ExecuteAsync(command, context, …)` with the now-populated handler.
5. Writes whatever the handler queued into `context.AddEvent` as a single transactional batch.

In the `CreateCommandHandler` above, the `Consume(AddedEvent, …)` overload sets `created = true`, so by the time `ExecuteAsync` runs the handler knows whether the customer has already been created and avoids emitting a duplicate `AddedEvent`. That is how idempotency is implemented in this library — there is no separate "is this already done?" lookup; the answer is in the event stream and the handler walks it.

State replay reads the entire stream every time. For long streams this matters — it's a cost worth being aware of when designing aggregates.

### Asynchronous Event Consumption

When state replay needs to call out to other services (lookups, validations, hydration), handlers can implement `IConsumeEventAsync<TEvent>` instead of `IConsumeEvent<TEvent>`:

```csharp
public Task ConsumeAsync(
    AddedEvent evt,
    EventMetadata metadata,
    CancellationToken cancellationToken)
{
    created = true;
    // external lookups here
    return Task.CompletedTask;
}
```

The pipeline awaits each `ConsumeAsync` call before moving to the next event, so order is preserved.

### Executing Commands

Commands are dispatched through `ICommandProcessorFactory`, which resolves a typed `ICommandProcessor<TCommand>`:

```csharp
var processor = factory.Create<CreateCommand>();
var result = await processor.ExecuteAsync(command, cancellationToken);
```

`CommandResult.ResultType` reports the outcome:

| Result Type    | Description                                                |
| -------------- | ---------------------------------------------------------- |
| `Changed`      | The command produced new events.                           |
| `NotModified`  | The command was a no-op for the current state.             |
| `Exists`       | The aggregate already existed in the requested state.      |
| `NotFound`     | The aggregate did not exist when the command required it.  |
| `Conflict`     | A concurrency conflict was hit and retries were exhausted. |

`CommandResult.ResponseObject` carries whatever the handler set on `context.ResponseObject`.

### Optimistic Concurrency and Conflict Resolution

Two clients can send commands targeting the same stream at the same time. The library detects this via the stream metadata document's ETag (see [Stream Metadata](#stream-metadata)) — it never locks.

A conflict happens when:

1. Command A reads the stream's events and metadata at version *N*.
2. Command B does the same, runs faster, and commits a write that bumps the stream to version *N+1*.
3. Command A's `TransactionalBatch` then tries to commit — but the ETag it captured no longer matches, and Cosmos rejects the batch.

How the pipeline reacts is controlled by the command's `Behavior` property:

| `OnConflict` value | What the pipeline does on conflict                                                                                                  | When to use                                                                                                            |
| ------------------ | ----------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `Fail` *(default)* | Surface the conflict. Returns `CommandResult.ResultType = Conflict`. The caller decides what to do.                                 | When the caller wants explicit control — e.g. retry policies live in the application, or conflicts must reach the user. |
| `Retry`            | Retry the **same write** (same events, same expected version) up to `BehaviorCount` times. Does **not** re-read or re-run the handler. | Transient infrastructure failures (throttling, network blips), or appends with `RequiredVersion = Any` where a stale read isn't a problem. |
| `RerunCommand`     | Throw away the handler, re-read the stream, replay all events into a fresh handler (now seeing the racer's events), call `ExecuteAsync` again, write. Up to `BehaviorCount` times. | When the handler's decision depends on stream state — typically anything with an idempotency check via `IConsumeEvent<TEvent>`. |

A simple example shows why the difference matters. Suppose `CreateCustomer` consumes `IConsumeEvent<CustomerCreated>` to skip its work if the customer already exists, and two clients race to create the same customer:

- Under `Retry`, the losing command keeps re-attempting the same write with the same expected version. The expected version is now stale, so every retry fails. Result: `Conflict`.
- Under `RerunCommand`, the losing command re-reads the stream, sees the `CustomerCreated` written by the winner, the idempotency guard kicks in, no events are queued, and the result is `NotModified`.

`Retry` is therefore not a substitute for `RerunCommand`. Pick based on whether the handler's decision can change once it sees what the racer wrote.

## Projections

### What a Projection Is

A projection is a worker that reads events from streams and turns them into something useful for the read side: a denormalised document, a search index entry, a cache row, an integration message. It is the "read" half of CQRS, and the only way the read side gets to know about new events.

A projection is intentionally separate from the command pipeline:

- **Eventually consistent.** A successful command does not block on the projection catching up.
- **Asynchronous.** Projections run as `IHostedService`s in the background.
- **Restartable.** Their position in the change feed is checkpointed, so they resume cleanly after a crash or deployment.
- **Replayable.** Drop the lease, and the projection replays from the beginning, rebuilding the read model from scratch.

That last point is the one that keeps surprising people: read models are not "the truth", they are a *cached view* of the events. If the schema changes or a bug corrupts the data, you fix the projection code and re-run.

### Defining a Projection

A projection implements `IProjection` plus one or more `IConsumeEvent<TEvent>` interfaces, and is filtered to a subset of streams via `[ProjectionFilter]`:

```csharp
[ProjectionFilter(SampleEventStreamId.FilterIncludeAllEvents)]
public class SampleProjection(
    ICosmosReader<SampleReadModel> reader,
    ICosmosWriter<SampleReadModel> writer) :
    IProjection,
    IConsumeEvent<AddedEvent>,
    IConsumeEvent<NameChangedEvent>,
    IConsumeEvent<AddressChangedEvent>,
    IConsumeEvent<DeletedEvent>
{
    private SampleReadModel view = null!;
    private bool deleted;

    public async Task InitializeAsync(EventStreamId id, CancellationToken cancellationToken)
    {
        var streamId = new SampleEventStreamId(id);
        view = await reader.FindAsync(streamId.Id, streamId.Id, cancellationToken)
            ?? new SampleReadModel { Id = streamId.Id };
    }

    public Task CompleteAsync(CancellationToken cancellationToken)
        => deleted
            ? writer.TryDeleteAsync(view.Id, view.PartitionKey, cancellationToken)
            : writer.WriteAsync(view, cancellationToken);

    public Task<ProjectionAction> FailedAsync(Exception exception, CancellationToken cancellationToken)
        => Task.FromResult(ProjectionAction.Continue);

    public void Consume(AddedEvent evt, EventMetadata metadata)
    {
        view.Name = evt.Name;
        view.Address = evt.Address;
        deleted = false;
    }

    // NameChangedEvent / AddressChangedEvent / DeletedEvent handlers omitted for brevity
}
```

The lifecycle is:

1. `InitializeAsync` is called once per processed stream batch. The projection loads the existing read-model document or creates a new one.
2. Each matching event is dispatched to the corresponding `Consume` overload, in stream order.
3. `CompleteAsync` persists the resulting read model.
4. `FailedAsync` decides whether the change-feed processor should continue past an error (`ProjectionAction.Continue`) or halt (`ProjectionAction.Stop`).

A new projection instance is resolved from DI for each batch, so per-instance fields (`view`, `deleted` above) are isolated between streams.

### How Projections Run

Under the hood each projection is hosted by the **Cosmos DB Change Feed Processor**:

- The processor watches the `event-store` container for new events.
- It coordinates with sibling instances (across replicas of your service) using **lease documents** in the `subscriptions` container — only one instance processes a given partition at a time, and leases are renewed to detect crashes.
- Each lease persists a continuation token: where in the feed this projection has read up to. After a restart the processor picks up exactly where it left off.
- The library groups events from the same stream into a batch and runs the projection lifecycle (`InitializeAsync` → `Consume` per event → `CompleteAsync`) for that batch. This is what makes "load read model once, mutate, save once" efficient even for streams with many events arriving in quick succession.

The projection's name (`nameof(SampleProjection)` in the registration below) becomes the lease name, so renaming a projection is the same as creating a new one — it will start from scratch unless you give it a `WithProjectionStartsFrom` checkpoint.

### Projection Filters

Filters use a wildcard syntax over the dot-separated parts of the stream id and are evaluated *before* events are dispatched, so unrelated streams don't pay the cost of waking the projection up:

| Pattern         | Matches                                          |
| --------------- | ------------------------------------------------ |
| `*`             | A stream with exactly one part.                  |
| `sample.*`      | All `sample.<id>` streams.                       |
| `sample.**`     | `sample.<id>` and any deeper stream beneath it.  |
| `sample.*.foo`  | A stream like `sample.<id>.foo`.                 |

The change feed itself still delivers every event in the container — the filter is applied client-side in the projection runner. This is why filters should be as narrow as possible: it's the projection's only line of defence against busy unrelated streams.

### Why Projections Cannot Write Events

Projections can dispatch commands, call external services, publish messages, or write to any storage they want. There is exactly one thing they cannot do: append events directly to a stream. The reason is causality.

A projection runs on the change feed *after* events are committed. If a projection wrote new events back to the event store:

- Those events would re-appear in the change feed, triggering the projection again.
- That second pass might emit yet more events, leading to feedback loops or non-deterministic state depending on timing.
- The audit trail would mix "events that record what the user did" with "events the projection synthesised", blurring the source of truth.

When a projection needs to record a fact in the event log, it dispatches a *command* through `ICommandProcessorFactory`. The command goes through the normal write pipeline — concurrency check, handler validation, atomic write — and the resulting events flow back to the projection (and any other projections) through the change feed. Causality stays one-way; the audit trail stays clean.

### Read-Model Storage

Read models do not have to live in the same Cosmos database as the events. Projections receive whatever services they need through DI — typical setups use `ICosmosReader<T>` and `ICosmosWriter<T>` from `Atc.Cosmos`, but any persistence (a different Cosmos container, SQL, blob storage, search index, …) is fair game.

A projection's write should be **idempotent**. The change feed processor offers at-least-once delivery, which means the same event can be re-delivered after a crash before its lease commit. Writing the read model with a deterministic id (typically the stream id) and a `WriteAsync`/upsert that accepts the same input twice without harm is the standard pattern.

### Registering Projections

Projections are registered with a unique name and run as background `IHostedService`s:

```csharp
builder.UseCQRS(c =>
{
    c.AddProjectionJob<SampleProjection>(nameof(SampleProjection));
});
```

The name must be stable — it identifies the lease that tracks the projection's position. Changing the name effectively starts a new projection from scratch.

### Starting From a Specific Date

For systems with long event histories, a projection can be told where to start in time so it does not replay irrelevant history:

```csharp
c.AddProjectionJob<SampleProjection>(nameof(SampleProjection))
 .WithProjectionStartsFrom(SubscriptionStartOptions.FromDateTime(new DateTime(2024, 1, 1)));
```

This applies only the first time the projection runs (when its lease document is created). Subsequent runs always resume from the lease's continuation token regardless of this setting.

## In-Memory Event Store

For unit and integration tests, swap the Cosmos DB backend for the in-memory implementation:

```csharp
services.AddEventStore(builder =>
{
    builder.UseInMemoryDb();
    builder.UseEvents(c => c.FromAssembly<AddedEvent>());
    builder.UseCQRS(c =>
    {
        c.AddCommandsFromAssembly<CreateCommand>();
    });
});
```

The same command, handler, and projection registrations work without changes, so the same code paths are exercised in tests as in production.

## Sample

The `sample/` folder contains a complete end-to-end example, split across four projects so the same domain types drive both a console worker and an HTTP API:

| Project                       | Role                                                                                                  |
| ----------------------------- | ----------------------------------------------------------------------------------------------------- |
| `GettingStarted.Domain`       | Class library containing the events, commands, handlers, projection, read model, and the shared `AddSampleEventStore` composition used by both apps. |
| `GettingStarted`              | Console worker (`IHostedService`) that issues a sequence of commands on startup.                      |
| `GettingStarted.WebApi`       | Minimal-API web service exposing `POST /customers`, `GET /customers/{id}`, `PUT /customers/{id}/name`, `DELETE /customers/{id}`. |
| `GettingStarted.AppHost`      | [.NET Aspire](https://aspire.dev) AppHost that orchestrates the Web API and the console worker against a Cosmos DB Emulator. |

Both apps share a single setup — `builder.Services.AddSampleEventStore(builder.Configuration)` — which wires up the Atc.Cosmos read-model store and the event store against the emulator.

### Prerequisite: the Azure Cosmos DB Emulator

The sample connects to the **locally-installed [Azure Cosmos DB Emulator](https://learn.microsoft.com/azure/cosmos-db/how-to-develop-emulator)** at `https://localhost:8081` (the apps fall back to that endpoint, using the well-known emulator key). **Start the emulator before launching the AppHost.**

The AppHost deliberately does **not** provision a Cosmos emulator container. The cross-platform Linux *preview* emulator is feature-incomplete — its transactional-batch responses omit the written content, which makes event writes fail — so the sample relies on the full local emulator instead. If you point the sample at a different emulator instance, give it the endpoint and key via the `CosmosOptions__AccountEndpoint` / `CosmosOptions__AccountKey` environment variables (the same `CosmosOptions` configuration section the apps bind).

Run the whole stack with the Aspire CLI:

```bash
aspire start --apphost sample/GettingStarted.AppHost
```

The Aspire dashboard (printed on startup) shows the Web API and the console worker:

- The **Web API** starts automatically. The dashboard shows a **Scalar** link (served at `/scalar/v1`) — the easiest way to try the endpoints interactively. You can also use `curl`:

  ```bash
  curl -k -X POST https://localhost:<port>/customers -H "Content-Type: application/json" -d '{"name":"Alice","address":"1 Main St"}'
  curl -k https://localhost:<port>/customers/<id>
  ```

- The **console worker** is registered with `WithExplicitStart()`, so it stays idle until you press **Start** in the dashboard — handy for demonstrating command issuance and projection updates on demand.

> When opted into the preview emulator container (`--UseEmulatorContainer true`), its Data Explorer is surfaced on the dashboard and data persists in a volume across runs — but event writes currently fail against it, so it is only useful for inspecting read models.

# Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) (for building and running tests; the libraries themselves target `netstandard2.1`)

# How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)