using System.Text.Json;
using RateLimiter.Reader.ConsumerService.Models;

namespace RateLimiter.Reader.ConsumerService.Mappers;

public static class MessageMapper
{
    public static MessageModel? FromJsonToModel(string message)
    {
        var jsonMessage = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(message);
        if (jsonMessage is null || !jsonMessage.ContainsKey("user_id") || !jsonMessage.ContainsKey("endpoint"))
        {
            return null;
        }

        var userId = jsonMessage["user_id"].GetInt32();
        var route = jsonMessage["endpoint"].GetString() ?? string.Empty;
        return new MessageModel(userId, route);
    }
}