using Atc.Test;
using Azure.Core;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests;

public class EventStoreClientOptionsTests
{
    [Fact]
    internal void Should_Default_ToCosmosEmulator()
    {
        var options = new EventStoreClientOptions();

        options.AuthKey.Should().Be(EventStoreClientOptions.EmulatorAuthKey);
        options.Endpoint.Should().Be(EventStoreClientOptions.EmulatorEndpoint);
        options.AllowAnyServerCertificate.Should().BeFalse();
        options.Credential.Should().BeNull();
    }

    [Fact]
    internal void Should_UseAuthKeyAndEndpoint()
    {
        var options = new EventStoreClientOptions();
        options.UseCredentials("endpoint", "auth-key");

        options.AuthKey.Should().Be("auth-key");
        options.Endpoint.Should().Be("endpoint");
        options.Credential.Should().BeNull();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_UseCredentialToken(
        TokenCredential token)
    {
        var options = new EventStoreClientOptions();
        options.UseCredentials("endpoint", token);

        options.Endpoint.Should().Be("endpoint");
        options.Credential.Should().Be(token);
        options.AuthKey.Should().BeNull();
    }

    [Fact]
    internal void Should_AllowAnyServerCertificate_When_UsingEmulator()
    {
        var options = new EventStoreClientOptions();
        options.UseCosmosEmulator(allowAnyServerCertificate: true);

        options.AuthKey.Should().Be(EventStoreClientOptions.EmulatorAuthKey);
        options.Endpoint.Should().Be(EventStoreClientOptions.EmulatorEndpoint);
        options.AllowAnyServerCertificate.Should().BeTrue();
        options.Credential.Should().BeNull();
    }

    [Fact]
    internal void Should_ConfigureCustomEndpointPort_When_UsingEmulator()
    {
        var options = new EventStoreClientOptions();
        options.UseCosmosEmulator("https://localhost:10222/");

        options.AuthKey.Should().Be(EventStoreClientOptions.EmulatorAuthKey);
        options.Endpoint.Should().Be("https://localhost:10222/");
        options.AllowAnyServerCertificate.Should().BeFalse();
        options.Credential.Should().BeNull();
    }
}