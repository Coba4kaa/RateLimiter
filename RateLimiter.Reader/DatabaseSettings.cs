namespace RateLimiter.Reader;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DbName { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
    public int BatchSize { get; set; } = 1000;
}
