using KafkaRedis.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaRedis.Domain.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaRedisConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(
            configuration.GetSection(nameof(KafkaSettings)).Bind);
            
        services.Configure<RedisSettings>(
            configuration.GetSection(nameof(RedisSettings)).Bind);

        return services;
    }
}
