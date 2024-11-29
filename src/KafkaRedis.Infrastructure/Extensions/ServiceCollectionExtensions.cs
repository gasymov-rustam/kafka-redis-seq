using KafkaRedis.Domain.Extensions;
using KafkaRedis.Domain.Interfaces;
using KafkaRedis.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaRedis.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaRedisServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddKafkaRedisConfiguration(configuration);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddSingleton<IKafkaProducer, KafkaProducerService>();
        services.AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
        services.AddSingleton<IKafkaConsumer, KafkaConsumerService>();
        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<IMessageSend, MessageSend>();

        services.AddHostedService<MessageTimerService>();

        return services;
    }
}
