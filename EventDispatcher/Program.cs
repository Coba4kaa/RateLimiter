using EventDispatcher;
using EventDispatcher.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var cts = new CancellationTokenSource();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<KafkaSettings>(context.Configuration.GetSection("KafkaSettings"));
        services.AddSingleton<EventDispatcher.Dispatchers.EventDispatcher>();
        services.AddHostedService<EventDispatcherHostedService>();
    })
    .Build();

var eventDispatcher = host.Services.GetRequiredService<EventDispatcher.Dispatchers.EventDispatcher>();

eventDispatcher.ConfigureEvent(123, "v1.0/users/getById", 10);
eventDispatcher.ConfigureEvent(123, "v1.0/users/getByName", 30);

var hostTask = host.RunAsync(cts.Token);

while (true)
{
    Console.WriteLine("Enter command (add/update id route rpm | changeroute id oldroute newroute | exit): ");
    var command = Console.ReadLine();
    var parts = command?.Split(' ');

    if (parts == null)
    {
        Console.WriteLine("Invalid command format.");
        continue;
    }

    var action = parts[0].ToLower();
    if (action == "exit")
    {
        Console.WriteLine("Exiting...");
        cts.Cancel();
        await host.StopAsync();
        break;
    }

    if (parts.Length < 2 || !int.TryParse(parts[1], out var userId))
    {
        Console.WriteLine("Invalid user ID format.");
        continue;
    }

    string route;
    int rpm;

    switch (action)
    {
        case "add":
            if (parts.Length != 4 || !int.TryParse(parts[3], out rpm))
            {
                Console.WriteLine("Invalid command format for add.");
                break;
            }

            route = parts[2];
            eventDispatcher.ConfigureEvent(userId, route, rpm);
            Console.WriteLine($"User {userId} added with route {route} and RPM {rpm}");
            break;

        case "update":
            if (parts.Length != 4 || !int.TryParse(parts[3], out rpm))
            {
                Console.WriteLine("Invalid command format for update.");
                break;
            }

            route = parts[2];
            eventDispatcher.ConfigureEvent(userId, route, rpm);
            Console.WriteLine($"User {userId} updated with route {route} and RPM {rpm}");
            break;

        case "changeroute":
            if (parts.Length != 4)
            {
                Console.WriteLine("Invalid command format for changeroute.");
                break;
            }

            var oldRoute = parts[2];
            var newRoute = parts[3];
            var routeChanged = eventDispatcher.ChangeRoute(userId, oldRoute, newRoute);

            if (routeChanged)
                Console.WriteLine($"User {userId}'s route changed from {oldRoute} to {newRoute}");
            break;

        default:
            Console.WriteLine("Invalid action. Use 'add', 'update', 'changeroute' or 'exit'.");
            break;
    }
}

await hostTask;