using MediatR;

namespace KafkaRedis.Domain.Commands;

public record ConsumeMessageCommand(string Topic) : IRequest;
