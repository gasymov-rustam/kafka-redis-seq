using KafkaRedis.Domain.Commands;
using KafkaRedis.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaRedis.Infrastructure.Services;

public class MessageTimerService : BackgroundService
{
    private readonly IKafkaProducer _kafkaProducer;
    private readonly IMessageSend _messageSend;
    private readonly ILogger<MessageTimerService> _logger;
    private readonly Random _random = new();

    public MessageTimerService(
        IKafkaProducer kafkaProducer,
        IMessageSend messageSend,
        ILogger<MessageTimerService> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
        _messageSend = messageSend;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var topics = _kafkaProducer.GetTopics();
                if (topics.Any())
                {
                    var randomTopic = topics[_random.Next(topics.Count)];
                    var message = $"Test message {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
                    
                    await _messageSend.SendMessage(randomTopic, message, stoppingToken);
                    _logger.LogInformation("Timer triggered: Message sent to topic {Topic}", randomTopic);
                }
                else
                {
                    _logger.LogWarning("No topics available for message production");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in message timer service");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
