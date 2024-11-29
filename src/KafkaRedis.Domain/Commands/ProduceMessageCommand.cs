using MediatR;

namespace KafkaRedis.Domain.Commands;

public record ProduceMessageCommand(string Topic, string Message) : IRequest;
