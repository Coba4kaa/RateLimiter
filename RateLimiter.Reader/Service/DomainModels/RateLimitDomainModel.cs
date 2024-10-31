namespace RateLimiter.Reader.Service.DomainModels;

public record RateLimitDomainModel(string Route, int RequestsPerMinute);