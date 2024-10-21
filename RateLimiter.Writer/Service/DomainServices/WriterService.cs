using RateLimiter.Writer.Repository;
using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Service.DomainServices;

public class WriterService : IWriterService {
    private readonly IRateLimitRepository _rateLimitRepository;

    public WriterService(IRateLimitRepository rateLimitRepository)
    {
        _rateLimitRepository = rateLimitRepository;
    }
    
    public Task<bool> CreateRateLimit(RateLimitDomainModel rateLimitDomainModel, CancellationToken cancellationToken)
    {
        return _rateLimitRepository.CreateAsync(rateLimitDomainModel, cancellationToken);
    }

    public Task<RateLimitDomainModel?> GetRateLimitByRoute(string route, CancellationToken cancellationToken)
    {
        return _rateLimitRepository.GetByRouteAsync(route, cancellationToken);
    }

    public Task<bool> UpdateRateLimit(RateLimitDomainModel rateLimitDomainModel, CancellationToken cancellationToken)
    {
        return _rateLimitRepository.UpdateAsync(rateLimitDomainModel, cancellationToken);
    }

    public Task<bool> DeleteRateLimit(string route, CancellationToken cancellationToken)
    {
        return _rateLimitRepository.DeleteAsync(route, cancellationToken);
    }
}