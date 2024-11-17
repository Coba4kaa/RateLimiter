using System.Text.Json.Serialization;

namespace EventDispatcher.Events
{
    public record UserEventPayload
    {
        [JsonPropertyName("user_id")]
        public int UserId { get; init; }

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; init; }

        public UserEventPayload(int userId, string endpoint)
        {
            UserId = userId;
            Endpoint = endpoint;
        }
    }
}