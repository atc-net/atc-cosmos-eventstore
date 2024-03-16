using System.Text.Json.Serialization;
using Atc.Cosmos;

namespace GettingStarted.Storage;

public class SampleReadModel : CosmosResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("pk")]
    public string PartitionKey { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    protected override string GetDocumentId() => Id;

    protected override string GetPartitionKey() => PartitionKey;
}