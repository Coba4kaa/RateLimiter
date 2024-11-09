using RateLimiter.Reader;
using RateLimiter.Reader.Controller;
using RateLimiter.Reader.Repository;
using RateLimiter.Reader.Service.DomainServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.AddSingleton<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();
builder.Services.AddSingleton<RateLimitGrpcController>();
builder.Services.AddHostedService<RateLimitHostedService>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<RateLimitGrpcController>();

await app.RunAsync("http://*:5000");