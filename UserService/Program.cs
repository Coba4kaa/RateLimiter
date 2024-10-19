using FluentValidation;
using Microsoft.Extensions.Options;
using UserService;
using UserService.Controller;
using UserService.Controller.Factory;
using UserService.Repository;
using UserService.Repository.DbModels;
using UserService.Service.DomainModel;
using UserService.Service.DomainService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddTransient<IUserFactory, UserFactory>();
builder.Services.AddTransient<IUserConverter, UserConverter>();
builder.Services.AddSingleton<IUserRepository>(provider =>
{
    var dbSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var userConverter = provider.GetRequiredService<IUserConverter>();
    var logger = provider.GetRequiredService<ILogger<UserRepository>>();
    return new UserRepository(dbSettings.UserServiceDb, userConverter, logger);
});

builder.Services.AddTransient<IUserService, UserService.Service.DomainService.UserService>();
builder.Services.AddTransient<IValidator<User>, UserValidator>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UsergRpcController>();

await app.RunAsync("http://*:5002");