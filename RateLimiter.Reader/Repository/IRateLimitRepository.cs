using System.Collections.ObjectModel;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Repository;

public interface IRateLimitRepository
{
    public Task<Dictionary<string, RateLimitDomainModel>> GetRateLimitsBatchAsync();
    public Task WatchRateLimitChangesAsync();
    public Collection<RateLimitDomainModel> GetCurrentRateLimits();
}