using System.Collections.Concurrent;
using System.Collections.ObjectModel;   
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Repository;

public interface IRateLimitRepository
{
    public Collection<RateLimitDomainModel> GetCurrentRateLimits();
    public void StartProcessingEvents();
}