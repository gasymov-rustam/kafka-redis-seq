namespace KafkaRedis.Domain.Interfaces;

public interface IKafkaProducer
{
    Task ProduceMessageAsync(string topic, string message);
    List<string> GetTopics();
    void UpdateTopics(List<string> topics);
}
