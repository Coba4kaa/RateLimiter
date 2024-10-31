using Microsoft.Extensions.Options;
using RateLimiter.Reader;
using RateLimiter.Reader.Controller;
using RateLimiter.Reader.Repository;
using RateLimiter.Reader.Service.DomainServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddSingleton<IRateLimitRepository>(provider => new RateLimitRepository(provider.GetRequiredService<IOptions<DatabaseSettings>>()));
builder.Services.AddSingleton<IRateLimitService, RateLimitService>();
builder.Services.AddSingleton<RateLimitGrpcController>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<RateLimitGrpcController>();

using (var scope = app.Services.CreateScope())
{
    var rateLimitRepository = scope.ServiceProvider.GetRequiredService<IRateLimitRepository>();
    await rateLimitRepository.GetRateLimitsBatchAsync();
    _ = Task.Run(() => rateLimitRepository.WatchRateLimitChangesAsync());
}

await app.RunAsync("http://*:5000");