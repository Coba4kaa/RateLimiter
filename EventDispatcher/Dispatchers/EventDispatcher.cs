using System.Collections.Concurrent;
using Confluent.Kafka;
using EventDispatcher.Events;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EventDispatcher.Dispatchers
{
    public class EventDispatcher : IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly ConcurrentDictionary<(int, string), UserEventConfig> _userEvents = new();
        private readonly ConcurrentDictionary<(int, string), CancellationTokenSource> _activeUserTasks = new();
        private readonly string _topicName;

        public EventDispatcher(IOptions<KafkaSettings> kafkaSettings)
        {
            var config = kafkaSettings.Value;

            if (string.IsNullOrEmpty(config.BootstrapServers))
                throw new ArgumentException("Kafka BootstrapServers is not configured.");

            if (string.IsNullOrEmpty(config.TopicName))
                throw new ArgumentException("Kafka TopicName is not configured.");

            _topicName = config.TopicName;

            var producerConfig = new ProducerConfig { BootstrapServers = config.BootstrapServers };
            _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
        }

        public void ConfigureEvent(int userId, string endpoint, int rpm)
        {
            var key = (userId, endpoint);
            RemoveExistingTask(key);

            var userEvent = new UserEventPayload(userId, endpoint);
            var userEventConfig = new UserEventConfig(userEvent, rpm);

            _userEvents[key] = userEventConfig;

            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => StartSending(key, cancellationTokenSource.Token), cancellationTokenSource.Token);
            _activeUserTasks[key] = cancellationTokenSource;
        }

        public bool ChangeRoute(int userId, string oldEndpoint, string newEndpoint)
        {
            if (oldEndpoint.Equals(newEndpoint, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine($"No route change needed for user {userId}. The endpoint '{newEndpoint}' is the same as the current one.");
                return false;
            }

            var oldKey = (userId, oldEndpoint);
            if (!_userEvents.TryGetValue(oldKey, out var userEventConfig))
            {
                Console.WriteLine($"Old route not found for user {userId} and endpoint {oldEndpoint}");
                return false;
            }

            var rpm = userEventConfig.Rpm;
            RemoveExistingTask(oldKey);
            ConfigureEvent(userId, newEndpoint, rpm);
            return true;
        }
        
        private void RemoveExistingTask((int, string) key)
        {
            if (_activeUserTasks.TryRemove(key, out var cts))
            {
                cts.Cancel();
            }
            _userEvents.TryRemove(key, out _);
        }

        private async Task StartSending((int, string) key, CancellationToken stoppingToken)
        {
            if (!_userEvents.TryGetValue(key, out var eventConfig)) return;

            while (!stoppingToken.IsCancellationRequested)
            {
                var jsonMessage = JsonConvert.SerializeObject(eventConfig.UserEvent);
                try
                {
                    await _producer.ProduceAsync(_topicName, new Message<Null, string> { Value = jsonMessage },
                        stoppingToken);
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine("Error sending message: {0}", e.Error.Reason);
                }

                await Task.Delay(60000 / eventConfig.Rpm, stoppingToken);
            }
        }

        public async Task StartBatchProcessing(CancellationToken stoppingToken)
        {
            foreach (var key in _userEvents.Keys)
            {
                if (_activeUserTasks.ContainsKey(key)) continue;

                var cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(() => StartSending(key, cancellationTokenSource.Token), cancellationTokenSource.Token);
                _activeUserTasks[key] = cancellationTokenSource;
            }

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public void Dispose()
        {
            _producer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}