using System.Text.Json.Serialization;
using Kontent.Ai.Delivery.Abstractions;
using Kontent.Ai.Delivery.SharedModels;

namespace DeliveryDemo.Models;

/// <summary>
/// Represents a hosted video content item for inline content resolution.
/// </summary>
public record HostedVideo
{
    [JsonPropertyName("video_id")]
    public string? VideoId { get; init; }

    [JsonPropertyName("video_host")]
    public IEnumerable<MultipleChoiceOption>? VideoHost { get; init; }
}
