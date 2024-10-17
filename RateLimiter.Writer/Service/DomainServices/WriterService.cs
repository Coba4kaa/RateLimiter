using MongoDB.Driver.Core.Operations;
using RateLimiter.Writer.Repository;
using RateLimiter.Writer.Service.DomainModels;
using RateLimiter.Writer.Service.Mappers;

namespace RateLimiter.Writer.Service.DomainServices;

public class WriterService : IWriterService {
    private readonly IRateLimitRepository _rateLimitRepository;

    public WriterService(IRateLimitRepository rateLimitRepository)
    {
        _rateLimitRepository = rateLimitRepository;
    }
    
    public async Task<bool> CreateRateLimitAsync(RateLimitDomainModel rateLimitDomainModel, CancellationToken cancellationToken)
    {
        return await _rateLimitRepository.CreateAsync(rateLimitDomainModel, cancellationToken);
    }

    public async Task<RateLimitDomainModel?> GetRateLimitByRouteAsync(string route, CancellationToken cancellationToken)
    {
        return await _rateLimitRepository.GetByRouteAsync(route, cancellationToken);
    }

    public async Task<bool> UpdateRateLimitAsync(RateLimitDomainModel rateLimitDomainModel, CancellationToken cancellationToken)
    {
        return await _rateLimitRepository.UpdateAsync(rateLimitDomainModel, cancellationToken);
    }

    public async Task<bool> DeleteRateLimitAsync(string route, CancellationToken cancellationToken)
    {
        return await _rateLimitRepository.DeleteAsync(route, cancellationToken);
    }
}