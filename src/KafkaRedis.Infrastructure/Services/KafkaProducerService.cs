using Confluent.Kafka;
using KafkaRedis.Domain.Interfaces;
using KafkaRedis.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KafkaRedis.Infrastructure.Services;

public class KafkaProducerService : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly IMediator _mediator;
    private readonly List<string> _topics;

    public KafkaProducerService(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaProducerService> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
        _topics = settings.Value.Topics;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task ProduceMessageAsync(string topic, string message)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            _logger.LogInformation("Message sent to topic {Topic} at partition {Partition} with offset {Offset}",
                topic, result.Partition.Value, result.Offset.Value);

            // await _mediator.Publish(new MessageReceivedEvent(topic, message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error producing message to topic {Topic}", topic);
            throw;
        }
    }

    public List<string> GetTopics() => _topics.ToList();

    public void UpdateTopics(List<string> topics)
    {
        _topics.Clear();
        _topics.AddRange(topics);
        _logger.LogInformation("Topics updated: {Topics}", string.Join(", ", topics));
    }
}
