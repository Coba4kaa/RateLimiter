using Confluent.Kafka;

using Microsoft.Extensions.Options;
using RateLimiter.Reader.ControlService;
using RateLimiter.Reader.ConsumerService.Mappers;


namespace RateLimiter.Reader.ConsumerService;

public class KafkaConsumerService : IDisposable
{
    private readonly IConsumer<Null, string> _consumer;
    private readonly IRequestControlService _requestControlService;
    private readonly string _topicName;

    public KafkaConsumerService(IOptions<KafkaSettings> kafkaSettings, IRequestControlService requestControlService)
    {
        var kafkaConfig = kafkaSettings.Value;
        
        if (string.IsNullOrEmpty(kafkaConfig.BootstrapServers))
            throw new ArgumentException("Kafka BootstrapServers is not configured.");

        if (string.IsNullOrEmpty(kafkaConfig.TopicName))
            throw new ArgumentException("Kafka TopicName is not configured.");
        
        var config = new ConsumerConfig
        {
            BootstrapServers = kafkaConfig.BootstrapServers,
            GroupId = kafkaConfig.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        
        _consumer = new ConsumerBuilder<Null, string>(config).Build();
        _topicName = kafkaConfig.TopicName;
        _requestControlService = requestControlService;
    }

    public async Task ConsumeMessages(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topicName);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var message = consumeResult.Message.Value;
                await HandleRequestAsync(message);
            }
        }
        catch (ConsumeException e)
        {
            Console.WriteLine($"Error while consuming message: {e.Error.Reason}");
        }
    }

    private async Task HandleRequestAsync(string message)
    {
        try
        {
            var messageModel = MessageMapper.FromJsonToModel(message);
            if (messageModel == null)
            {
                Console.WriteLine($"Invalid message format, can't handle request.");
                return;
            }
            var userId = messageModel.UserId;
            var route = messageModel.Route;
            if (string.IsNullOrEmpty(route))
            {
                Console.WriteLine("Invalid message route, can't handle request.");
                return;
            }

            Console.WriteLine($"Received request for {route} from user {userId}");
            await _requestControlService.ProcessRequestAsync(messageModel);
        }

        catch (Exception e)
        {
            Console.WriteLine($"Error serializing json: {e}");
        }
    }
    
    public void Dispose()
    {
        _consumer.Dispose();
    }
}