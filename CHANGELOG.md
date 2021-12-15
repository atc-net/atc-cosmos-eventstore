# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

-   Support for Token Credentials with Comos DB using `UseCredentials` methods on options class.

### Deprecated

-   EventStore `ConnectionString` option has been made obsolete, please use `UseCredentials` or `UseCosmosEmulator` instead.

## [1.1.3] - 2021-11-16

### Added

-   Test

[Unreleased]: https://github.com/atc-net/atc-cosmos-eventstore/compare/v1.1.3...HEAD

[1.1.3]: https://github.com/atc-net/atc-cosmos-eventstore/compare/1d853dcb043e906a8b92c4b74f113dc676419233...1.1.3
