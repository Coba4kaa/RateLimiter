using UserService.Repository.interfaces;
using StackExchange.Redis;

namespace UserService.Repository;

public class RateLimitRepository(IConnectionMultiplexer redis) : IRateLimitRepository
{
    public async Task<bool> IsRateLimitExceededAsync(int userId, string methodName)
    {
        var redisDb = redis.GetDatabase();
        var exceededKey = $"has_exceeded_rpm:{userId}:{methodName}";
        return await redisDb.KeyExistsAsync(exceededKey);
    }
}
