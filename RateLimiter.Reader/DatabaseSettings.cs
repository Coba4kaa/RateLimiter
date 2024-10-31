namespace RateLimiter.Reader;

public class DatabaseSettings
{
    public string RateLimiterDb { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 1000;
}
