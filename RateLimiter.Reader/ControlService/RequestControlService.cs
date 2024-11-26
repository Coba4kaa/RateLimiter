using Microsoft.Extensions.Options;
using RateLimiter.Reader.ConsumerService.Models;
using RateLimiter.Reader.Service.DomainServices;
using StackExchange.Redis;

namespace RateLimiter.Reader.ControlService;

public class RequestControlService : IRequestControlService, IDisposable
{
    private IDatabase? _redisDb;
    private ConnectionMultiplexer? _redisConnection;
    private readonly RedisSettings _redisSettings;
    private readonly IRateLimitService _rateLimitService;
    private readonly TimeSpan _blockDuration;
    private readonly TimeSpan _counterDuration;

    public RequestControlService(IOptions<RedisSettings> redisSettings, IRateLimitService rateLimitService)
    {
        _redisSettings = redisSettings.Value;
        _rateLimitService = rateLimitService;
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
    
    public async Task ProcessRequestAsync(MessageModel messageModel)
    {
        var userId = messageModel.UserId;
        var route = messageModel.Route;
        var routeRateLimitModel = _rateLimitService.FindRateLimitForRoute(route);
        if (routeRateLimitModel is null)
        {
            Console.WriteLine($"Current route {route} has no rate limit. User {userId} request allowed freely.");
            return;
        }
        var routeRateLimit = routeRateLimitModel.RequestsPerMinute;
        var redisKey = $"route_request:{userId}:{route}";
        var exceededKey = $"has_exceeded_rpm:{userId}:{route}";
        var isBlocked = await _redisDb.KeyExistsAsync(exceededKey);
        if (isBlocked)
        {
            Console.WriteLine($"User {userId} is in timeout for route {route}. Access blocked.");
            return;
        }
        
        var currentCount = await _redisDb.StringIncrementAsync(redisKey);
        if (currentCount == 1)
        {
            await _redisDb.KeyExpireAsync(redisKey, _counterDuration);
        }

        if (currentCount > routeRateLimit)
        {
            await _redisDb.StringSetAsync(exceededKey, 1, _blockDuration);
            await _redisDb.KeyDeleteAsync(redisKey);
            Console.WriteLine($"Rate limit exceeded for UserID: {userId}, Route: {route}. Access blocked.");
        }
    }
    
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        _redisDb = await InitializeRedisAsync(cancellationToken);
    }

    public void Dispose()
    {
        _redisConnection?.Dispose();
    }

    private async Task<IDatabase> InitializeRedisAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_redisSettings.ConnectionString))
        {
            throw new ArgumentException("Redis connection string is not configured.");
        }

        try
        {
            var connectTask = ConnectionMultiplexer.ConnectAsync(_redisSettings.ConnectionString);
            await Task.WhenAny(connectTask, Task.Delay(Timeout.Infinite, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
            var redisConnection = await connectTask;
            _redisConnection = redisConnection;
            return _redisConnection.GetDatabase();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Redis connection was canceled.");
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to connect to Redis. Please check your connection settings.", ex);
        }
    }
}
