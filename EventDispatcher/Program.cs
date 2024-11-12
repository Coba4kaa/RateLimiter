using Confluent.Kafka;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9093"
        };

        try
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var message = new Message<Null, string>
                {
                    Value = "Hello Kafka"
                };
                
                var deliveryResult = await producer.ProduceAsync("events", message);
                Console.WriteLine($"Message delivered to {deliveryResult.TopicPartitionOffset}");
            }
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Error: {e.Error.Reason}");
        }
    }
}