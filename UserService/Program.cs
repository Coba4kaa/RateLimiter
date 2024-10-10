using FluentValidation;
using UserService.Controller;
using UserService.Repository;
using UserService.Service;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("UserServiceDb");
builder.Services.AddSingleton<IUserRepository>(provider => new UserRepository(connectionString));
builder.Services.AddTransient<IValidator<User>, UserValidator>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<UsergRpcController>();

await app.RunAsync("http://*:5002");