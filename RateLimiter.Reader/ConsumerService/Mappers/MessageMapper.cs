using System.Text.Json;
using RateLimiter.Reader.ConsumerService.Models;

namespace RateLimiter.Reader.ConsumerService.Mappers;

public static class MessageMapper
{
    public static MessageModel? FromJsonToModel(string message)
    {
        var jsonMessage = JsonSerializer.Deserialize<MessageModel>(message);
        if (jsonMessage is null || string.IsNullOrEmpty(jsonMessage.Route))
        {
            return null;
        }
        
        return jsonMessage;
    }
}