using MongoDB.Driver;
using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Service.DomainServices
{
    public interface IRateLimitService
    {
        public IReadOnlyCollection<RateLimitDomainModel> GetCurrentRateLimits();
        public Task InitializeRateLimitsAsync();
        public Task ProcessRateLimitChangesAsync(IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> changes);
    }
}