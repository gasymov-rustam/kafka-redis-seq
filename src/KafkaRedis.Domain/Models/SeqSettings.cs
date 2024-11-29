namespace KafkaRedis.Domain.Models;

public class SeqSettings
{
    public string ServerUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string MinimumLevel { get; set; } = "Information";
    public Dictionary<string, string> LevelOverride { get; set; } = new();
}
