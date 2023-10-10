# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.12.6] - 2023-10-10

### Fixed

-   `RequiredVersion` was not respected when command handler was consuming events.

## [1.11.3] - 2023-09-08

-   Add missing methods in `IProjectionBuilder` to enable configuration of projection subscription start options.

## [1.10.3] - 2023-09-05

### Added

-   Support for controlling the start time from where a new event subscription or projection should start receiving changes. By default, it will start from the beginning of time to preserve backwards compatibility.

```csharp
services.AddEventStore(builder =>
{
    builder.UseCosmosDb();
    builder.UseEvents(c => c.FromAssembly<MyEvent>());
    builder.UseCQRS(c =>
    {
        c.AddProjectionJob<MyProjection>(
            "super-projection-name",
            c => c.WithProjectionStartsFrom(
                SubscriptionStartOptions.FromBegining)); // setting the start time
    });
});
```

-   Controlling projection `PollingInterval` and `MaxItems` received on ever iteration.

## [1.9.17] - 2023-07-03

### Added

-   Introduced options to provide events async when testing commands using `ICommandGiven`, `ICommandWhen` and `ICommandThen`.
-   Introduce hard limits to the number of events the system can accept per operation. 
    -   A maximum of 10 events per command context (CQRS)
    -   A maximum of 50 events per stream batch (Event Store)

### Fixed

-   Fixed issue where adding two or more projections with the same class name would override their configurations resulting in the "dead" projections.

## [1.8.3] - 2023-06-02

### Fixed

-   Removed writing to stream-index when a new stream is created.

## [1.7.23] - 2023-05-31

### Added

-   Pipeline for controlling event data convertion `IEventDataConverter`
-   Added custom event data converters to be configured using `EventStoreOptions`. This will enable scenarioes such as converting from one version of an event to another.
-   Unknown or invalid events can now be observed through the `IConsumeEvent<T>` and `IConsumeEventAsync<T>` by using well known types `FaultedEvent` and `UnknownEvent`.
-   Introduced new interfaces `IConsumeAnyEvent` and `IConsumeAnyEventAsync` for consuming any event without specifying it type.
-   Command processor is now registered as singleton, eliminating the need for using ICommandProcessorFactory.
-   Optionally configure cosmos client to accept any server certificate when using emulator.

### Fixed

-   Raise condition when 2 command processors tries to add the first event to the same stream concurrently.
-   Rerunning command now create a new instance of the command processor to clear out any previous state it might contain.

### Removed

-   Setting `ConfigurationString` when configuring event store options.
-   `EventId` has been removed from `Metadata`.

## [1.6.8] - 2022-07-06

### Added

-   Exception delegate for receiving any exception douing a stream subscription.
-   Throws `ArgumentException` when a projection is missing a `ProjectionFilter`.
-   **BREAKING** - `IProjection` now require you to implement `FailedAsync(Exception exception,
    CancellationToken cancellationToken)` and instruct the framework on how to proceed when encountering an exception.
-   Convenience extension methods to CommandContext.

## [1.5.3] - 2022-07-05

### Added

-   Introduced configuration of custom json converters (#23)

## [1.4.5] - 2022-03-18

### Changed

-   Fixed issue where projection will not start do to missing dependency registration.
-   Enhanced documentation for `EventStoreClientOptions` and fix spelling.
-   Dependencies for `Microsoft.Azure.Cosmos` has been upgraded from `3.23.0` to `3.26.1`.
-   Dependencies for ` System.Text.Json` has been upgraded from `6.0.1` to `6.0.2`.

## [1.3.3] - 2022-01-31

### Changed

-   Fixed issue where using `UseCredentials` when configuring event store would not work.

## [1.2.9] - 2022-01-30

### Added

-   Support for Token Credentials with Comos DB using `UseCredentials` methods on options class.

### Deprecated

-   EventStore `ConnectionString` option has been made obsolete, please use `UseCredentials` or `UseCosmosEmulator` instead.

## [1.1.3] - 2021-11-16

### Added

-   Support for Token Credentials with Comos DB using `UseCredentials` methods on options class.

### Deprecated

-   EventStore `ConnectionString` option has been made obsolete, please use `UseCredentials` or `UseCosmosEmulator` instead.

[Unreleased]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.12.6...HEAD

[1.12.6]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.11.3...v1.12.6

[1.11.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.10.3...v1.11.3

[1.10.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.9.17...v1.10.3

[1.9.17]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.8.3...v1.9.17

[1.8.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.7.23...v1.8.3

[1.7.23]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.6.8...v1.7.23

[1.6.8]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.5.3...v1.6.8

[1.5.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.4.5...v1.5.3

[1.4.5]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.3.3...v1.4.5

[1.3.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.2.9...v1.3.3

[1.2.9]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.1.3...v1.2.9
