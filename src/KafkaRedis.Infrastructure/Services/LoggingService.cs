using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Serilog;
using KafkaRedis.Domain.Models;

namespace KafkaRedis.Infrastructure.Services;

public interface ILoggingService
{
    void ConfigureLogging(IConfiguration configuration);
}

public class LoggingService : ILoggingService
{
    private readonly SeqSettings _seqSettings;

    public LoggingService(IOptions<SeqSettings> seqSettings)
    {
        _seqSettings = seqSettings.Value;
    }

    public void ConfigureLogging(IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.Seq(_seqSettings.ServerUrl)
            .Enrich.FromLogContext()
            .CreateLogger();
    }
}
