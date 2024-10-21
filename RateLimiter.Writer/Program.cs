using FluentValidation;
using Microsoft.Extensions.Options;
using RateLimiter.Writer;
using RateLimiter.Writer.Controller;
using RateLimiter.Writer.Controller.Validators;
using RateLimiter.Writer.Repository;
using RateLimiter.Writer.Service.DomainModels;
using RateLimiter.Writer.Service.DomainServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<IRateLimitRepository>(provider =>
{
    var dbSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new RateLimitRepository(dbSettings.RateLimiterDb);
});

builder.Services.AddTransient<IWriterService, WriterService>();
builder.Services.AddTransient<RateLimitValidator>();
builder.Services.AddTransient<IValidator<RateLimitDomainModel>, RateLimitValidator>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<WriterGrpcController>();

await app.RunAsync("http://*:5001");