namespace RateLimiter.Reader.ControlService;

public class RequestControlHostedService(IRequestControlService requestControlService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Redis initializing...");
        await requestControlService.InitializeAsync(cancellationToken);
        Console.WriteLine("Redis initialized.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Stopping hosted service...");
        return Task.CompletedTask;
    }
}