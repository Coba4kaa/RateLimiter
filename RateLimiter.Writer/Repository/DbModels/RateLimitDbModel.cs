using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RateLimiter.Writer.Repository.DbModels;

public record RateLimitDbModel
{
    [BsonId]
    public ObjectId Id { get; init; } = ObjectId.GenerateNewId();

    public string Route { get; init; } = string.Empty;
    public int RequestsPerMinute { get; init; }

};
