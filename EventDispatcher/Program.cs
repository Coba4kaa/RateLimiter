using EventDispatcher;
using EventDispatcher.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

while (true)
{
    Console.WriteLine("Enter command (add/update id route rpm | changeroute id oldroute newroute): ");
    var command = Console.ReadLine();
    var parts = command?.Split(' ');

    if (parts == null || parts.Length < 3)
    {
        Console.WriteLine("Invalid command format.");
        continue;
    }

    var action = parts[0].ToLower();
    if (!int.TryParse(parts[1], out var userId))
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
            eventDispatcher.ChangeRoute(userId, oldRoute, newRoute);
            Console.WriteLine($"User {userId}'s route changed from {oldRoute} to {newRoute}");
            break;

        default:
            Console.WriteLine("Invalid action. Use 'add', 'update', or 'changeroute'.");
            break;
    }
}