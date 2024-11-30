using Grpc.Core;
using Grpc.Core.Interceptors;
using UserService.Service.DomainService.interfaces;

namespace UserService.Interceptors;

public class RateLimitInterceptor(IRateLimitService rateLimitService) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        int.TryParse(context.RequestHeaders.GetValue("user_id"), out var userId);
        var methodName = context.Method;

        await rateLimitService.CheckRateLimitAsync(userId, methodName);

        return await continuation(request, context);
    }
}