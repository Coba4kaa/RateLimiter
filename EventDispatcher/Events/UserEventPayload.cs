using Newtonsoft.Json;

namespace EventDispatcher.Events;

public record UserEventPayload(
    [property: JsonProperty("user_id")] int UserId, 
    [property: JsonProperty("endpoint")] string Endpoint);