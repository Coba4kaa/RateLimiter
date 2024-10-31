using System.Collections.ObjectModel;
using RateLimiter.Reader.Repository;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Service.DomainServices
{
    public class RateLimitService(IRateLimitRepository rateLimitRepository) : IRateLimitService
    {
        public IReadOnlyCollection<RateLimitDomainModel> GetCurrentRateLimits()
        {
            return rateLimitRepository.GetCurrentRateLimits();
        }
    }
}