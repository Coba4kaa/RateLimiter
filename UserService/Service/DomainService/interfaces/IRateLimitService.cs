namespace UserService.Service.DomainService.interfaces;

public interface IRateLimitService
{
    public Task<bool> IsRateLimitExceededAsync(int userId, string methodName);
}