using KafkaRedis.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKafkaRedisServices(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
