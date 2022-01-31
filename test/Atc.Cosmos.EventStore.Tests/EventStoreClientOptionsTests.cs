using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atc.Test;
using Azure.Core;
using FluentAssertions;
using Xunit;

namespace Atc.Cosmos.EventStore.Tests
{
    public class EventStoreClientOptionsTests
    {
        [Fact]
        internal void Should_Default_ToCosmosEmulator()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var options = new EventStoreClientOptions();

            options.ConnectionString.Should().Be(EventStoreClientOptions.CosmosEmulatorConnectionString);
#pragma warning restore CS0618 // Type or member is obsolete
            options.AuthKey.Should().BeNull();
            options.Endpoint.Should().BeNull();
            options.Credential.Should().BeNull();
        }

        [Fact]
        internal void Should_Set_ConnectionString()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var options = new EventStoreClientOptions
            {
                ConnectionString = "connection-string",
            };

            options.ConnectionString.Should().Be("connection-string");
#pragma warning restore CS0618 // Type or member is obsolete
            options.AuthKey.Should().BeNull();
            options.Endpoint.Should().BeNull();
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
#pragma warning disable CS0618 // Type or member is obsolete
            options.ConnectionString.Should().BeNull();
#pragma warning restore CS0618 // Type or member is obsolete
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
#pragma warning disable CS0618 // Type or member is obsolete
            options.ConnectionString.Should().BeNull();
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}