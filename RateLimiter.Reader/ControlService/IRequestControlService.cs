using RateLimiter.Reader.ConsumerService.Models;

namespace RateLimiter.Reader.ControlService;

public interface IRequestControlService : IDisposable
{
    public Task InitializeAsync(CancellationToken cancellationToken);
    public Task ProcessRequestAsync(MessageModel messageModel);
}