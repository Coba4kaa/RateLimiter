using System.Text.Json.Serialization;

namespace RateLimiter.Reader.ConsumerService.Models;

public record MessageModel
{
    [JsonPropertyName("user_id")]
    public int UserId { get; init; }

    [JsonPropertyName("endpoint")]
    public required string Route { get; init; }
}
