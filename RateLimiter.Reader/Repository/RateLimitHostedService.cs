namespace RateLimiter.Reader.Repository
{
    public class RateLimitHostedService(IRateLimitRepository rateLimitRepository) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            rateLimitRepository.StartProcessingEvents();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}