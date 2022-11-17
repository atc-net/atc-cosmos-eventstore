using System;

namespace Atc.Cosmos.EventStore.Tests.Fakes;

public record EventOne(string Name, int Number);

public record EventTwo(string Name, DateTimeOffset Timestamp);

public record EventThree(string Name);
