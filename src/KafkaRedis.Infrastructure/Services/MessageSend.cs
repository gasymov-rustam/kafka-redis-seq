using KafkaRedis.Domain.Commands;
using KafkaRedis.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KafkaRedis.Infrastructure.Services;

public class MessageSend: IMessageSend
{
    private readonly IMediator _mediator;
    private readonly ILogger<MessageSend> _logger;

    public MessageSend(IMediator meditor, ILogger<MessageSend> logger)
    {
        _mediator = meditor;
        _logger = logger;
    }

    public async Task SendMessage(string topic, string message, CancellationToken token = default )
    {
        try
        {
            await _mediator.Send(new ProduceMessageCommand(topic, message), token);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error sending message to topic {Topic}", topic);
            throw;
        }
    }
}
