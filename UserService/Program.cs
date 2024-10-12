using FluentValidation;
using Microsoft.Extensions.Options;
using UserService;
using UserService.Controller;
using UserService.Controller.Factory;
using UserService.Repository;
using UserService.Service.DomainModel;
using UserService.Service.DomainService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddSingleton<IUserRepository>(provider =>
{
    var dbSettings = provider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new UserRepository(dbSettings.UserServiceDb);
});

builder.Services.AddTransient<IUserService, UserService.Service.DomainService.UserService>();
builder.Services.AddTransient<IValidator<User>, UserValidator>();
builder.Services.AddTransient<IUserFactory, UserFactory>();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UsergRpcController>();

await app.RunAsync("http://*:5002");