using RateLimiter.Reader;
using RateLimiter.Reader.ConsumerService;
using RateLimiter.Reader.Controller;
using RateLimiter.Reader.ControlService;
using RateLimiter.Reader.Repository;
using RateLimiter.Reader.Service.DomainServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddSingleton<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();
builder.Services.AddHostedService<RateLimitHostedService>();
builder.Services.AddHostedService<RequestControlHostedService>();
builder.Services.AddHostedService<KafkaConsumerHostedService>();
builder.Services.AddSingleton<KafkaConsumerService>();
builder.Services.AddSingleton<IRequestControlService, RequestControlService>();
builder.Services.AddSingleton<RateLimitGrpcController>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<RateLimitGrpcController>();

await app.RunAsync("http://*:5000");