using EventDispatcher;
using FluentValidation;
using StackExchange.Redis;
using UserService;
using UserService.Controller;
using UserService.Controller.Factory;
using UserService.Interceptors;
using UserService.Repository;
using UserService.Repository.DbModels;
using UserService.Service.DomainModel;
using UserService.Service.DomainService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<IUserDomainControllerConverter, UserDomainControllerConverter>();
builder.Services.AddSingleton<IUserDomainDbConverter, UserDomainDbConverter>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();

builder.Services.AddSingleton<IUserService, UserService.Service.DomainService.UserService>();
builder.Services.AddSingleton<IValidator<User>, UserValidator>();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));
builder.Services.AddSingleton<EventDispatcher.Dispatchers.EventDispatcher>();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<AuthenticationInterceptor>();
    options.Interceptors.Add<RateLimitInterceptor>();
});


var app = builder.Build();

app.MapGrpcService<UsergRpcController>();

await app.RunAsync("http://*:5002");