using UserService.Repository.interfaces;
using UserService.Service.DomainService.interfaces;

namespace UserService.Service.DomainService;

public class RateLimitService(IRateLimitRepository rateLimitRepository,
    EventDispatcher.Dispatchers.EventDispatcher eventDispatcher) : IRateLimitService
{
    public async Task<bool> IsRateLimitExceededAsync(int userId, string methodName)
    {
        if (await rateLimitRepository.IsRateLimitExceededAsync(userId, methodName))
            return true;
        
        eventDispatcher.ConfigureEvent(userId, methodName, 0);
        return false;
    }
}
