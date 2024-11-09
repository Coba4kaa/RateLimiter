using System.Collections.Concurrent;
using MongoDB.Driver;
using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Repository;

public interface IRateLimitRepository
{
    public Task<ConcurrentDictionary<string, RateLimitDomainModel>> GetRateLimitsBatchAsync();
    public IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchRateLimitChangesAsync();
}