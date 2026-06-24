namespace Atc.Cosmos.EventStore.Tests.Cosmos;

public sealed class CosmosEventSerializerTests
{
    private readonly CosmosEventSerializer sut;

    public CosmosEventSerializerTests()
    {
        var options = Options.Create(new EventStoreClientOptions());
        var typeProvider = Substitute.For<IEventTypeProvider>();

        sut = new CosmosEventSerializer(options, typeProvider);
    }

    [Fact]
    internal void Should_Deserialize_When_Stream_Buffer_Is_Not_Exposable()
    {
        // Arrange
        // A MemoryStream created from a byte[] is NOT publicly exposable,
        // so TryGetBuffer returns false. This mirrors the response stream
        // shape returned by Microsoft.Azure.Cosmos 3.57.1.
        var json = "{\"name\":\"hello\"}"u8.ToArray();
        using var stream = new System.IO.MemoryStream(json);

        // Act
        var result = sut.FromStream<Poco>(stream);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("hello");
    }

    [Fact]
    internal void Should_Deserialize_When_Stream_Is_Not_A_MemoryStream()
    {
        // Arrange
        // The SDK does not guarantee the response is a MemoryStream.
        var json = "{\"name\":\"world\"}"u8.ToArray();
        using var stream = new NonSeekableStream(json);

        // Act
        var result = sut.FromStream<Poco>(stream);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("world");
    }

    internal sealed class Poco
    {
        public string? Name { get; set; }
    }

    private sealed class NonSeekableStream : System.IO.Stream
    {
        private readonly System.IO.MemoryStream inner;

        public NonSeekableStream(byte[] bytes)
            => inner = new System.IO.MemoryStream(bytes);

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(
            byte[] buffer,
            int offset,
            int count)
            => inner.Read(buffer, offset, count);

        public override void Flush()
            => throw new NotSupportedException();

        public override long Seek(
            long offset,
            System.IO.SeekOrigin origin)
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(
            byte[] buffer,
            int offset,
            int count)
            => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                inner.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}