using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Service.DomainServices;

public interface IWriterService
{
    Task<bool> CreateRateLimitAsync(RateLimitDomainModel rateLimit, CancellationToken cancellationToken);
    Task<RateLimitDomainModel?> GetRateLimitByRouteAsync(string route, CancellationToken cancellationToken);
    Task<bool> UpdateRateLimitAsync(RateLimitDomainModel rateLimit, CancellationToken cancellationToken);
    Task<bool> DeleteRateLimitAsync(string route, CancellationToken cancellationToken);
}