namespace UserService.Repository.interfaces;

public interface IRateLimitRepository
{
    public Task<bool> IsRateLimitExceededAsync(int userId, string methodName);
}