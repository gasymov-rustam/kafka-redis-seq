namespace KafkaRedis.Domain.Interfaces;

public interface IMessageSend
{
    public Task SendMessage(string topic, string message, CancellationToken token = default);
}
