namespace KafkaRedis.Domain.Interfaces;

public interface IRedisService
{
    Task SetAsync(string key, string value);
    Task<string?> GetAsync(string key);
    Task UpdateTopicsAsync(List<string> topics);
}
