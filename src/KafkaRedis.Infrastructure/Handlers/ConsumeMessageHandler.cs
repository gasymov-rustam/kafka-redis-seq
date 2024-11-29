using KafkaRedis.Domain.Commands;
using KafkaRedis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KafkaRedis.Infrastructure.Handlers;

public class ConsumeMessageHandler : IRequestHandler<ConsumeMessageCommand>
{
    private readonly IKafkaConsumer _consumer;
    private readonly ILogger<ConsumeMessageHandler> _logger;

    public ConsumeMessageHandler(
        IKafkaConsumer consumer,
        ILogger<ConsumeMessageHandler> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public async Task Handle(ConsumeMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _consumer.ConsumeMessageAsync(request.Topic);
            _logger.LogInformation("Message consumed from topic {Topic}", request.Topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling consume message command for topic {Topic}", request.Topic);
            throw;
        }
    }
}
