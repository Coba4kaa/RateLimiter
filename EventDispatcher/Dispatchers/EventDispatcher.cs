using System.Collections.Concurrent;
using Confluent.Kafka;
using EventDispatcher.Events;
using Newtonsoft.Json;


namespace EventDispatcher.Dispatchers;

public class EventDispatcher : IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly ConcurrentDictionary<int, UserEventConfig> _userEvents = new();

    public EventDispatcher(string kafkaBootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = kafkaBootstrapServers};
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public void ConfigureEvent(int userId, string endpoint, int rpm)
    {
        var userEvent = new UserEventPayload(userId, endpoint);
        var userEventConfig = new UserEventConfig(userEvent, rpm);
        _userEvents[userId] = userEventConfig;
        
    }

    public async Task StartBatchProcessing(CancellationToken stoppingToken)
    {
        var activeUserTasks = new ConcurrentDictionary<int, Task>();

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var userEventConfig in _userEvents)
            {
                var userId = userEventConfig.Key;
                
                if (!activeUserTasks.ContainsKey(userId))
                {
                    var task = Task.Run(() => StartSending(userId, stoppingToken), stoppingToken);
                    activeUserTasks[userId] = task;
                }
            }
            
            await Task.Delay(5000, stoppingToken);
            
            foreach (var (userId, task) in activeUserTasks)
            {
                if (task.IsCompleted)
                {
                    activeUserTasks.TryRemove(userId, out _);
                }
            }
        }
        
        await Task.WhenAll(activeUserTasks.Values);
    }

    private async Task StartSending(int userId, CancellationToken stoppingToken)
    {
        if (!_userEvents.TryGetValue(userId, out var eventConfig)) return;
        var delay = 60000 / eventConfig.Rpm;
        while (!stoppingToken.IsCancellationRequested)
        {
            var jsonMessage = JsonConvert.SerializeObject(eventConfig.UserEvent);
            try
            {
                var deliveryResult =
                    await _producer.ProduceAsync("events", new Message<Null, string> { Value = jsonMessage });
            }

            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine("Error sending message: {0}", e.Error.Reason);
            }
            
            await Task.Delay(delay, stoppingToken);
        }
    }
    
    public void Dispose()
    {
        _producer?.Dispose();
        GC.SuppressFinalize(this);
    }
}