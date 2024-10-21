using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Repository;

public interface IRateLimitRepository {
    Task<bool> CreateAsync(RateLimitDomainModel rateLimit, CancellationToken cancellationToken);
    Task<RateLimitDomainModel?> GetByRouteAsync(string route, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(RateLimitDomainModel rateLimit, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string route, CancellationToken cancellationToken);
}