using Microsoft.Extensions.Hosting;

namespace EventDispatcher.Dispatchers;

public class EventDispatcherHostedService(EventDispatcher eventDispatcher) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await eventDispatcher.StartBatchProcessing(stoppingToken);
    }

    public override void Dispose()
    {
        eventDispatcher.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}
