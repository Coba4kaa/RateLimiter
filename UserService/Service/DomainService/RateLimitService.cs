using Grpc.Core;
using UserService.Repository;
using UserService.Repository.interfaces;
using UserService.Service.DomainService.interfaces;

namespace UserService.Service.DomainService;

public class RateLimitService(
    IRateLimitRepository rateLimitRepository,
    EventDispatcher.Dispatchers.EventDispatcher eventDispatcher) : IRateLimitService
{
    public async Task CheckRateLimitAsync(int userId, string methodName)
    {
        if (await rateLimitRepository.IsRateLimitExceededAsync(userId, methodName))
        {
            throw new RpcException(new Status(StatusCode.ResourceExhausted,
                "Rate limit exceeded. Please try again later."));
        }

        eventDispatcher.ConfigureEvent(userId, methodName, 0);
    }
}
