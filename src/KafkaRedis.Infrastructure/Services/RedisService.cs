using KafkaRedis.Domain.Interfaces;
using KafkaRedis.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace KafkaRedis.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly ILogger<RedisService> _logger;
    private readonly string _instanceName;
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private readonly TimeSpan _valueTimeToLive;
    private const string _topicsKey = "topics";

    public RedisService(
        IOptions<RedisSettings> settings,
        ILogger<RedisService> logger)
    {
        _logger = logger;
        _instanceName = settings.Value.InstanceName;
        _valueTimeToLive = settings.Value.ValueTimeToLive;
        _redis = ConnectionMultiplexer.Connect(settings.Value.ConnectionString);
        _db = _redis.GetDatabase();
    }

    public async Task SetAsync(string key, string value)
    {
        try
        {
            await _db.StringSetAsync($"{_instanceName}{key}", value, _valueTimeToLive);
            _logger.LogInformation("Successfully set value for key: {Key} with TTL: {TTL}", key, _valueTimeToLive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value for key: {Key}", key);
            throw;
        }
    }

    public async Task<string?> GetAsync(string key)
    {
        try
        {
            var value = await _db.StringGetAsync($"{_instanceName}{key}");
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value for key: {Key}", key);
            throw;
        }
    }

    public async Task UpdateTopicsAsync(List<string> topics)
    {
        try
        {
            var topicsString = string.Join(",", topics);
            await _db.StringSetAsync($"{_instanceName}{_topicsKey}", topicsString, _valueTimeToLive);
            _logger.LogInformation("Successfully updated topics in Redis: {Topics} with TTL: {TTL}", topicsString, _valueTimeToLive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating topics in Redis");
            throw;
        }
    }
}
