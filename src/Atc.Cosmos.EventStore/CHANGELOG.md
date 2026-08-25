# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0](https://github.com/atc-net/atc-cosmos-eventstore/compare/Atc.Cosmos.EventStore@v1.18.0...Atc.Cosmos.EventStore@v2.0.0) (2026-08-25)


### ⚠ BREAKING CHANGES

* **deps:** bump NuGet packages across solution
* The public members SubscriptionStartOptions.FromBegining and EventStreamId.PartSeperator have been renamed to FromBeginning and PartSeparator. Update references accordingly.
* **deps:** upgrade Cosmos 3.46.0->3.61.0 and Newtonsoft.Json 13.0.3->13.0.4

### chore

* **deps:** bump NuGet packages across solution ([4083b92](https://github.com/atc-net/atc-cosmos-eventstore/commit/4083b92a59a452d8e58d04fdc94606fa1b5e85f3))


### refactor

* fix public API typos and modernize source ([f623c3a](https://github.com/atc-net/atc-cosmos-eventstore/commit/f623c3a59a4323d3ee28760396510ef7ecd89140))


### New features

* **deps:** upgrade Cosmos 3.46.0-&gt;3.61.0 and Newtonsoft.Json 13.0.3-&gt;13.0.4 ([e03a509](https://github.com/atc-net/atc-cosmos-eventstore/commit/e03a509e54d6d5bb05d882fc9ebce74402378ad6))


### Performance improvements

* **eventstore:** avoid throwing CosmosException for not-found reads ([4b7036f](https://github.com/atc-net/atc-cosmos-eventstore/commit/4b7036fe5bf5b382fea3ff8b31ab9c14c162e4dd))

## [1.18.0](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.17.0...v1.18.0) (2025-04-08)

-   Released in lockstep with `Atc.Cosmos.EventStore.Cqrs` 1.18.0. No core-specific changes.

## [1.15.4](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.14.11...v1.15.4) (2024-12-23)

### Added

-   Remove `IEventStoreManagementClient` and move `DeleteStreamAsync` method to `IEventStoreClient`.

## [1.14.11](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.13.3...v1.14.11) (2024-12-09)

### Added

-   Implement `IEventStoreManagementClient.DeleteStreamAsync` using the newly released `DeleteAllItemsByPartitionKeyStreamAsync` method in the Cosmos SDK.

## [1.13.3](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.12.6...v1.13.3) (2024-04-21)

### Added

-   Introduce instrumentation support for Open Telemetry.

```csharp
builder
    .Services
    .AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();
    })
    .WithTracing(tracing =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // We want to view all traces in development
            tracing.SetSampler(new AlwaysOnSampler());
        }

        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(EventStoreDiagnostics.SourceName); // enable trace telemetry from event store and cqrs.
    });
```

## [1.10.3](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.9.17...v1.10.3) (2023-09-05)

### Added

-   Support for controlling the start time from where a new event subscription should start receiving changes. By default, it will start from the beginning of time to preserve backwards compatibility.

## [1.9.17](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.8.3...v1.9.17) (2023-07-03)

### Added

-   Introduce hard limits to the number of events the system can accept per operation.
    -   A maximum of 50 events per stream batch (Event Store)

## [1.8.3](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.7.23...v1.8.3) (2023-06-02)

### Fixed

-   Removed writing to stream-index when a new stream is created.

## [1.7.23](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.6.8...v1.7.23) (2023-05-31)

### Added

-   Pipeline for controlling event data convertion `IEventDataConverter`
-   Added custom event data converters to be configured using `EventStoreOptions`. This will enable scenarioes such as converting from one version of an event to another.
-   Unknown or invalid events can now be observed through the `IConsumeEvent<T>` and `IConsumeEventAsync<T>` by using well known types `FaultedEvent` and `UnknownEvent`.
-   Introduced new interfaces `IConsumeAnyEvent` and `IConsumeAnyEventAsync` for consuming any event without specifying it type.
-   Optionally configure cosmos client to accept any server certificate when using emulator.

### Fixed

-   Raise condition when 2 command processors tries to add the first event to the same stream concurrently.

### Removed

-   Setting `ConfigurationString` when configuring event store options.
-   `EventId` has been removed from `Metadata`.

## [1.6.8](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.5.3...v1.6.8) (2022-07-06)

### Added

-   Exception delegate for receiving any exception douing a stream subscription.

## [1.5.3](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.4.5...v1.5.3) (2022-07-05)

### Added

-   Introduced configuration of custom json converters (#23)

## [1.4.5](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.3.3...v1.4.5) (2022-03-18)

### Changed

-   Enhanced documentation for `EventStoreClientOptions` and fix spelling.
-   Dependencies for `Microsoft.Azure.Cosmos` has been upgraded from `3.23.0` to `3.26.1`.
-   Dependencies for `System.Text.Json` has been upgraded from `6.0.1` to `6.0.2`.

## [1.3.3](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.2.9...v1.3.3) (2022-01-31)

### Changed

-   Fixed issue where using `UseCredentials` when configuring event store would not work.

## [1.2.9](https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.1.3...v1.2.9) (2022-01-30)

### Added

-   Support for Token Credentials with Comos DB using `UseCredentials` methods on options class.

### Deprecated

-   EventStore `ConnectionString` option has been made obsolete, please use `UseCredentials` or `UseCosmosEmulator` instead.

## 1.1.3 (2021-11-16)

### Added

-   Support for Token Credentials with Comos DB using `UseCredentials` methods on options class.

### Deprecated

-   EventStore `ConnectionString` option has been made obsolete, please use `UseCredentials` or `UseCosmosEmulator` instead.
