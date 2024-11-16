namespace EventDispatcher;

public class KafkaSettings
{
    public string BootstrapServers { get; init; } = string.Empty;
    public string TopicName { get; init; } = string.Empty;
}
