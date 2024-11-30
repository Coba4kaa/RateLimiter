using EventDispatcher;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using UserService;
using UserService.Controller;
using UserService.Controller.Converter;
using UserService.Interceptors;
using UserService.Repository;
using UserService.Repository.DbModels;
using UserService.Service.DomainInterface;
using UserService.Service.DomainService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<IUserRequestControllerConverter, UserRequestControllerConverter>();
builder.Services.AddSingleton<IUserDbConverter, UserDbConverter>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService.Service.DomainService.UserService>();
builder.Services.AddSingleton<IValidator<IUser>, UserValidator>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddSingleton<EventDispatcher.Dispatchers.EventDispatcher>();
builder.Services.AddMemoryCache();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<AuthenticationInterceptor>();
    options.Interceptors.Add<RateLimitInterceptor>();
});


var app = builder.Build();

app.MapGrpcService<UsergRpcController>();

await app.RunAsync("http://*:5002");