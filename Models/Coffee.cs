using System.Text.Json.Serialization;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.ContentItems;
using Kontent.Ai.Delivery.SharedModels;

namespace DeliveryDemo.Models;

/// <summary>
/// Represents a coffee product from the Kontent.ai sample project.
/// </summary>
public record Coffee
{
    [JsonPropertyName("product_name")]
    public string? ProductName { get; init; }

    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    [JsonPropertyName("image")]
    public IEnumerable<Asset>? Image { get; init; }

    [JsonPropertyName("short_description")]
    public string? ShortDescription { get; init; }

    [JsonPropertyName("long_description")]
    public string? LongDescription { get; init; }

    [JsonPropertyName("processing")]
    public IEnumerable<TaxonomyTerm>? Processing { get; init; }

    [JsonPropertyName("product_status")]
    public IEnumerable<TaxonomyTerm>? ProductStatus { get; init; }

    [JsonPropertyName("country")]
    public string? Country { get; init; }

    [JsonPropertyName("farm")]
    public string? Farm { get; init; }

    [JsonPropertyName("variety")]
    public string? Variety { get; init; }

    [JsonPropertyName("altitude")]
    public string? Altitude { get; init; }
}
