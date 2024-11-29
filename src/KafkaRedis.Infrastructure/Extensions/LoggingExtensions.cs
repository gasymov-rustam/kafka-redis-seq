using KafkaRedis.Domain.Models;
using KafkaRedis.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace KafkaRedis.Infrastructure.Extensions;

public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<SeqSettings>(
            configuration.GetSection("Seq"));

        services.AddSingleton<ILoggingService, LoggingService>();
        var loggingService = services.BuildServiceProvider()
            .GetRequiredService<ILoggingService>();

        loggingService.ConfigureLogging(configuration);

        return services;
    }

    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
            };
        });

        return app;
    }
}
