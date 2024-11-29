using Confluent.Kafka;
using KafkaRedis.Domain.Interfaces;
using KafkaRedis.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaRedis.Infrastructure.Services;

public class KafkaConsumerFactory : IKafkaConsumerFactory
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaConsumerFactory> _logger;
    private readonly Dictionary<string, IConsumer<Null, string>> _consumers;

    public KafkaConsumerFactory(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaConsumerFactory> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        _consumers = new Dictionary<string, IConsumer<Null, string>>();
    }

    public IConsumer<Null, string> GetOrCreateConsumer(string topic)
    {
        if (_consumers.TryGetValue(topic, out var existingConsumer))
        {
            return existingConsumer;
        }

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = $"{_settings.GroupId}_{topic}",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
        consumer.Subscribe(topic);
        
        _consumers[topic] = consumer;
        _logger.LogInformation("Created new consumer for topic: {Topic}", topic);
        
        return consumer;
    }

    public void RemoveConsumer(string topic)
    {
        if (_consumers.TryGetValue(topic, out var consumer))
        {
            try
            {
                consumer.Close();
                consumer.Dispose();
                _consumers.Remove(topic);
                _logger.LogInformation("Removed and disposed consumer for topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing consumer for topic: {Topic}", topic);
            }
        }
    }

    /* public IReadOnlyCollection<string> GetActiveTopics()
    {
        return _consumers.Keys.ToList().AsReadOnly();
    } */

    /* public void UpdateSubscriptions(IEnumerable<string> topics)
    {
        var newTopics = topics.ToList();
        var currentTopics = _consumers.Keys.ToList();

        var topicsToRemove = currentTopics.Except(newTopics);
        foreach (var topic in topicsToRemove)
        {
            RemoveConsumer(topic);
        }

        var topicsToAdd = newTopics.Except(currentTopics);
        foreach (var topic in topicsToAdd)
        {
            GetOrCreateConsumer(topic);
        }

        _logger.LogInformation("Updated subscriptions. Active topics: {Topics}", 
            string.Join(", ", GetActiveTopics()));
    } */

    public void Dispose()
    {
        foreach (var (topic, consumer) in _consumers)
        {
            try
            {
                consumer.Close();
                consumer.Dispose();
                RemoveConsumer(topic);
                _logger.LogInformation("Disposed consumer for topic: {Topic}", topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing consumer for topic: {Topic}", topic);
            }
        }
        _consumers.Clear();
    }
}
