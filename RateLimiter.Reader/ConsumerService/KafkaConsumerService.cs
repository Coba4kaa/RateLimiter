using Confluent.Kafka;

using Microsoft.Extensions.Options;
using RateLimiter.Reader.ControlService;
using System.Text.Json;


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
            var jsonMessage = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(message);
            if (jsonMessage is null || !jsonMessage.ContainsKey("user_id") || !jsonMessage.ContainsKey("endpoint"))
            {
                Console.WriteLine("Invalid message format, can't handle request.");
                return;
            }

            var userId = jsonMessage["user_id"].GetInt32();
            var endpoint = jsonMessage["endpoint"].GetString() ?? string.Empty;
            if (string.IsNullOrEmpty(endpoint))
            {
                Console.WriteLine("Invalid message endpoint, can't handle request.");
                return;
            }

            Console.WriteLine($"Received request for {endpoint} from user {userId}");
            await _requestControlService.ProcessRequestAsync(userId, endpoint);
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