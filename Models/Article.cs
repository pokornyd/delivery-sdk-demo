using System.Text.Json.Serialization;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.ContentItems;
using Kontent.Ai.Delivery.ContentItems.RichText;
using Kontent.Ai.Delivery.SharedModels;

namespace DeliveryDemo.Models;

/// <summary>
/// Represents an article content item from the Kontent.ai sample project.
/// </summary>
public record Article
{
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    [JsonPropertyName("summary")]
    public string? Summary { get; init; }

    [JsonPropertyName("body_copy")]
    public RichTextContent? BodyCopy { get; init; }

    [JsonPropertyName("post_date")]
    public DateTime? PostDate { get; init; }

    [JsonPropertyName("teaser_image")]
    public IEnumerable<Asset>? TeaserImage { get; init; }

    [JsonPropertyName("related_articles")]
    public IEnumerable<string>? RelatedArticles { get; init; }

    [JsonPropertyName("personas")]
    public IEnumerable<TaxonomyTerm>? Personas { get; init; }
}
