# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.2.9...HEAD

[1.2.9]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.1.3...v1.2.9
