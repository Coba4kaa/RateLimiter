using Microsoft.Extensions.Hosting;

namespace EventDispatcher.Dispatchers;

public class EventDispatcherHostedService : BackgroundService
{
    private readonly EventDispatcher _eventDispatcher;

    public EventDispatcherHostedService(EventDispatcher eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _eventDispatcher.StartBatchProcessing(stoppingToken);
    }
}
