namespace RateLimiter.Reader;

public class RedisSettings{
    public string ConnectionString { get; init; } = string.Empty;
    public int DbIndex { get; init; }
    public string BlockDuration { get; init; } = string.Empty;
    public string CounterDuration { get; init; } = string.Empty;
}
