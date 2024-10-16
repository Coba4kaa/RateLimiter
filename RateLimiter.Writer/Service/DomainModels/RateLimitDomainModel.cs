namespace RateLimiter.Writer.Service.DomainModels;

public record RateLimitDomainModel(string Route, int RequestsPerMinute);