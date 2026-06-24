namespace GettingStarted.Projections;

public class SampleReadModel : CosmosResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("pk")]
    [SuppressMessage(
        "Naming",
        "CA1721:Property names should not match get methods",
        Justification = "Property is the JSON pk field; GetPartitionKey is forced by the CosmosResource base class.")]
    public string PartitionKey { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    protected override string GetDocumentId() => Id;

    protected override string GetPartitionKey() => PartitionKey;
}