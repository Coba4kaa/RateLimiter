namespace UserService.Service.DomainService.interfaces;

public interface IRateLimitService
{
    public Task CheckRateLimitAsync(int userId, string methodName);
}