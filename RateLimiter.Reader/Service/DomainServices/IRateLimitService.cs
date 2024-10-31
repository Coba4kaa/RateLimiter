using System.Collections.ObjectModel;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Service.DomainServices
{
    public interface IRateLimitService
    {
        public Collection<RateLimitDomainModel> GetCurrentRateLimits();
    }
}