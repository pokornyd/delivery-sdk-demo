using System.Text.Json.Serialization;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.SharedModels;

namespace DeliveryDemo.Models;

/// <summary>
/// Represents a tweet content item for inline content resolution.
/// </summary>
public record Tweet
{
    [JsonPropertyName("tweet_link")]
    public string? TweetLink { get; init; }

    [JsonPropertyName("theme")]
    public IEnumerable<MultipleChoiceOption>? Theme { get; init; }

    [JsonPropertyName("display_options")]
    public IEnumerable<MultipleChoiceOption>? DisplayOptions { get; init; }
}
