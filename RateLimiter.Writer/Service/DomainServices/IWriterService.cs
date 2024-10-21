using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Service.DomainServices;

public interface IWriterService
{
    Task<bool> CreateRateLimit(RateLimitDomainModel rateLimit, CancellationToken cancellationToken);
    Task<RateLimitDomainModel?> GetRateLimitByRoute(string route, CancellationToken cancellationToken);
    Task<bool> UpdateRateLimit(RateLimitDomainModel rateLimit, CancellationToken cancellationToken);
    Task<bool> DeleteRateLimit(string route, CancellationToken cancellationToken);
}