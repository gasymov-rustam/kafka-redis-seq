using MediatR;

namespace KafkaRedis.Domain.Events;

public record struct MessageReceivedEvent(string Topic, string Message) : INotification;
