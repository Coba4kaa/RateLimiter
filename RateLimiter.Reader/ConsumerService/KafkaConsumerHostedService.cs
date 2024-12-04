namespace RateLimiter.Reader.ConsumerService;

public class KafkaConsumerHostedService(KafkaConsumerService kafkaConsumerService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await kafkaConsumerService.ConsumeMessages(stoppingToken);
    }
    
    public override void Dispose()
    {
        kafkaConsumerService.Dispose();
        base.Dispose();
    }
}