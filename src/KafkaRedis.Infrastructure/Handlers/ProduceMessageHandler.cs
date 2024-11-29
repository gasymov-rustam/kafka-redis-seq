using KafkaRedis.Domain.Commands;
using KafkaRedis.Domain.Events;
using KafkaRedis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KafkaRedis.Infrastructure.Handlers;

public class ProduceMessageHandler : IRequestHandler<ProduceMessageCommand>
{
    private readonly IKafkaProducer _producer;
    private readonly IRedisService _redisService;
    private readonly IMediator _mediator;
    private readonly ILogger<ProduceMessageHandler> _logger;

    public ProduceMessageHandler(
        IKafkaProducer producer,
        IRedisService redisService,
        IMediator mediator,
        ILogger<ProduceMessageHandler> logger)
    {
        _producer = producer;
        _redisService = redisService;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(ProduceMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _producer.ProduceMessageAsync(request.Topic, request.Message);
            _logger.LogInformation("Message sent to Kafka topic {Topic}", request.Topic);

            await _redisService.SetAsync($"message_{request.Topic}", request.Message);
            _logger.LogInformation("Message saved to Redis for topic {Topic}", request.Topic);

            await _mediator.Send(new ConsumeMessageCommand(request.Topic), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling produce message command for topic {Topic}", request.Topic);
            throw;
        }
    }
}
