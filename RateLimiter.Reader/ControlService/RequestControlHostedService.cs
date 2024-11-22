namespace RateLimiter.Reader.ControlService;

public class RequestControlHostedService(IRequestControlService requestControlService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Redis initializing...");
        await requestControlService.InitializeAsync(cancellationToken);
        Console.WriteLine("Redis initialized.");
    }
    
    public override void Dispose()
    {
        requestControlService.Dispose();
        base.Dispose();
    }
}