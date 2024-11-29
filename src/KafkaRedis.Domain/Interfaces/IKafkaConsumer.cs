namespace KafkaRedis.Domain.Interfaces;

public interface IKafkaConsumer
{
    ValueTask ConsumeMessageAsync(string topic);
}
