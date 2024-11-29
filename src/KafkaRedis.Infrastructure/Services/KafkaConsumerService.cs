using Confluent.Kafka;
using KafkaRedis.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace KafkaRedis.Infrastructure.Services;

public class KafkaConsumerService : IKafkaConsumer, IDisposable
{
    private readonly IKafkaConsumerFactory _consumerFactory;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(
        IKafkaConsumerFactory consumerFactory,
        ILogger<KafkaConsumerService> logger)
    {
        _consumerFactory = consumerFactory;
        _logger = logger;
    }

    public async ValueTask ConsumeMessageAsync(string topic)
    {
        try
        {
            var consumer = _consumerFactory.GetOrCreateConsumer(topic);
            var consumeResult = consumer.Consume(TimeSpan.FromSeconds(10));
            
            if (consumeResult != null)
            {
                _logger.LogInformation("Message received from topic {Topic}: {Message}",
                    consumeResult.Topic, consumeResult.Message.Value);
                
                consumer.Commit(consumeResult);
            }
            else
            {
                _logger.LogDebug("No message received from topic {Topic} within timeout", topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming message from topic {Topic}", topic);
            throw;
        }
    }

    public void Dispose()
    {
        _consumerFactory.Dispose();
    }
}
