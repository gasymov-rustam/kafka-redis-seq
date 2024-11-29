using Confluent.Kafka;
using System.Collections.Generic;

namespace KafkaRedis.Domain.Interfaces;

public interface IKafkaConsumerFactory : IDisposable
{
    IConsumer<Null, string> GetOrCreateConsumer(string topic);
    void RemoveConsumer(string topic);
    // IReadOnlyCollection<string> GetActiveTopics();
    // void UpdateSubscriptions(IEnumerable<string> topics);
}
