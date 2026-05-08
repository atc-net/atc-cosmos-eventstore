namespace Atc.Cosmos.EventStore.Tests;

public sealed class EventStoreClientOptionsTests
{
    [Fact]
    internal void Should_Default_ToCosmosEmulator()
    {
        // Act
        var result = new EventStoreClientOptions();

        // Assert
        result.AuthKey.Should().Be(EventStoreClientOptions.EmulatorAuthKey);
        result.Endpoint.Should().Be(EventStoreClientOptions.EmulatorEndpoint);
        result.AllowAnyServerCertificate.Should().BeFalse();
        result.Credential.Should().BeNull();
    }

    [Fact]
    internal void Should_UseAuthKeyAndEndpoint()
    {
        // Arrange
        var options = new EventStoreClientOptions();

        // Act
        options.UseCredentials("endpoint", "auth-key");

        // Assert
        options.AuthKey.Should().Be("auth-key");
        options.Endpoint.Should().Be("endpoint");
        options.Credential.Should().BeNull();
    }

    [Theory, AutoNSubstituteData]
    internal void Should_UseCredentialToken(TokenCredential token)
    {
        // Arrange
        var options = new EventStoreClientOptions();

        // Act
        options.UseCredentials("endpoint", token);

        // Assert
        options.Endpoint.Should().Be("endpoint");
        options.Credential.Should().Be(token);
        options.AuthKey.Should().BeNull();
    }

    [Fact]
    internal void Should_AllowAnyServerCertificate_When_UsingEmulator()
    {
        // Arrange
        var options = new EventStoreClientOptions();

        // Act
        options.UseCosmosEmulator(allowAnyServerCertificate: true);

        // Assert
        options.AuthKey.Should().Be(EventStoreClientOptions.EmulatorAuthKey);
        options.Endpoint.Should().Be(EventStoreClientOptions.EmulatorEndpoint);
        options.AllowAnyServerCertificate.Should().BeTrue();
        options.Credential.Should().BeNull();
    }

    [Fact]
    internal void Should_ConfigureCustomEndpointPort_When_UsingEmulator()
    {
        // Arrange
        var options = new EventStoreClientOptions();

        // Act
        options.UseCosmosEmulator("https://localhost:10222/");

        // Assert
        options.AuthKey.Should().Be(EventStoreClientOptions.EmulatorAuthKey);
        options.Endpoint.Should().Be("https://localhost:10222/");
        options.AllowAnyServerCertificate.Should().BeFalse();
        options.Credential.Should().BeNull();
    }
}