using Grpc.Core;
using Grpc.Core.Interceptors;
using StackExchange.Redis;

namespace UserService.Interceptors;

public class RateLimitInterceptor(
    IConnectionMultiplexer redis,
    EventDispatcher.Dispatchers.EventDispatcher eventDispatcher) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        int.TryParse(context.RequestHeaders.GetValue("user_id"), out var userId);

        var methodName = context.Method;
        var redisDb = redis.GetDatabase();
        var exceededKey = $"has_exceeded_rpm:{userId}:{methodName}";

        var isBlocked = await redisDb.KeyExistsAsync(exceededKey);
        if (isBlocked)
        {
            throw new RpcException(new Status(StatusCode.ResourceExhausted,
                "Rate limit exceeded. Please try again later."));
        }

        eventDispatcher.ConfigureEvent(userId, methodName, 0);

        return await continuation(request, context);
    }
}