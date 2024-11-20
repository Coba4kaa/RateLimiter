namespace RateLimiter.Reader.ControlService;

public interface IRequestControlService
{
    public Task ProcessRequestAsync(int userId, string endpoint);
}