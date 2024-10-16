using RateLimiter.Writer.Repository.DbModels;
using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Repository;

public interface IRateLimitRepository {
    Task<bool> CreateAsync(RateLimitDbModel rateLimit, CancellationToken cancellationToken);
    Task<RateLimitDomainModel?> GetByRouteAsync(string route, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(RateLimitDbModel rateLimit, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(string route, CancellationToken cancellationToken);
}