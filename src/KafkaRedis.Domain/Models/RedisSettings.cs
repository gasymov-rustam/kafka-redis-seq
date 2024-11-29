namespace KafkaRedis.Domain.Models;

public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string InstanceName { get; set; } = string.Empty;
    public TimeSpan ValueTimeToLive { get; set; } = TimeSpan.FromHours(1);
}
