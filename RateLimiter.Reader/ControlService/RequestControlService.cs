using Microsoft.Extensions.Options;
using RateLimiter.Reader.Service.DomainModels;
using RateLimiter.Reader.Service.DomainServices;
using StackExchange.Redis;

namespace RateLimiter.Reader.ControlService;

public class RequestControlService : IRequestControlService
{
    private IDatabase? _redisDb;
    private readonly RedisSettings _redisSettings;
    private readonly IRateLimitService _rateLimitService;
    private readonly TimeSpan _blockDuration;
    private readonly TimeSpan _counterDuration;
    private IReadOnlyCollection<RateLimitDomainModel> _currentRateLimits;
    private bool _isInitialized;

    public RequestControlService(IOptions<RedisSettings> redisSettings, IRateLimitService rateLimitService)
    {
        _redisSettings = redisSettings.Value;
        _rateLimitService = rateLimitService;
        _currentRateLimits = rateLimitService.GetCurrentRateLimits();
        _blockDuration = TimeSpan.Parse(_redisSettings.BlockDuration);
        if (_blockDuration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(_blockDuration), "BlockDuration cannot be negative.");
        }
        
        _counterDuration = TimeSpan.Parse(_redisSettings.CounterDuration);
        if (_counterDuration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(_counterDuration), "CounterDuration cannot be negative.");
        }
    }
    
    public async Task ProcessRequestAsync(int userId, string endpoint)
    {
        if (!_isInitialized)
        {
            Console.WriteLine("Redis initializing...");
            await InitializeAsync();
            _isInitialized = true;
            Console.WriteLine("Redis initialized.");
        }
        _currentRateLimits = _rateLimitService.GetCurrentRateLimits();
        var endpointRateLimitModel = _currentRateLimits.FirstOrDefault(limit => limit.Route.Equals(endpoint, StringComparison.OrdinalIgnoreCase));
        if (endpointRateLimitModel is null)
        {
            Console.WriteLine($"Current endpoint {endpoint} has no rate limit. User request allowed freely.");
            return;
        }
        var endpointRateLimit = endpointRateLimitModel.RequestsPerMinute;
        var redisKey = $"endpoint_request:{userId}:{endpoint}";
        var exceededKey = $"has_exceeded_rpm:{userId}:{endpoint}";
        var endpointRateLimitKey = $"old_rate_limit:{endpoint}";
        var oldRateLimit = await _redisDb.StringGetAsync(endpointRateLimitKey);
        var isBlocked = await _redisDb.KeyExistsAsync(exceededKey);
        if (isBlocked)
        {
            Console.WriteLine($"Rate limit exceeded for UserID: {userId}, Endpoint: {endpoint}. Access blocked.");
            return;
        }

        if (oldRateLimit.HasValue && oldRateLimit != endpointRateLimit)
        {
            await _redisDb.StringSetAsync(redisKey, 1, _counterDuration);
            await _redisDb.StringSetAsync(endpointRateLimitKey, endpointRateLimit);
            return;
            
        }
        
        var currentCount = await _redisDb.StringIncrementAsync(redisKey);
        if (currentCount == 1)
        {
            await _redisDb.KeyExpireAsync(redisKey, _counterDuration);
        }

        if (currentCount >= endpointRateLimit)
        {
            await _redisDb.StringSetAsync(exceededKey, 1, _blockDuration);
            await _redisDb.KeyDeleteAsync(redisKey);

        }
        
        await _redisDb.StringSetAsync(endpointRateLimitKey, endpointRateLimit, TimeSpan.FromMinutes(10));
    }
    
    private async Task InitializeAsync()
    {
        _redisDb = await InitializeRedisAsync();
    }

    private async Task<IDatabase> InitializeRedisAsync()
    {
        if (string.IsNullOrEmpty(_redisSettings.ConnectionString))
        {
            throw new ArgumentException("Redis connection string is not configured.");
        }

        try
        {
            var redisConnection = await ConnectionMultiplexer.ConnectAsync(_redisSettings.ConnectionString);
            return redisConnection.GetDatabase();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to connect to Redis. Please check your connection settings.", ex);
        }
    }
}
