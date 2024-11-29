using Serilog;
using KafkaRedis.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLoggingServices(builder.Configuration);
builder.Host.UseSerilog();

builder.Services.AddKafkaRedisServices(builder.Configuration);

var app = builder.Build();

app.UseLoggingMiddleware();

app.UseHttpsRedirection();

app.Run();
