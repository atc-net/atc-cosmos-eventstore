# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Introduced configuration of custom json converters (#23)

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

[Unreleased]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.4.5...HEAD

[1.4.5]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.3.3...v1.4.5

[1.3.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.2.9...v1.3.3

[1.2.9]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.1.3...v1.2.9
