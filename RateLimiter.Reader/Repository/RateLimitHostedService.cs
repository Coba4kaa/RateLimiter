using RateLimiter.Reader.Service.DomainServices;

namespace RateLimiter.Reader.Repository
{
    public class RateLimitHostedService(IRateLimitService rateLimitService, IRateLimitRepository rateLimitRepository)
        : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await rateLimitService.InitializeRateLimitsAsync();
            var changes = rateLimitRepository.WatchRateLimitChangesAsync();
            _ = Task.Run(() => rateLimitService.ProcessRateLimitChangesAsync(changes), cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}