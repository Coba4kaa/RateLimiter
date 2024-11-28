using Grpc.Core;
using Grpc.Core.Interceptors;

namespace UserService.Interceptors;

public class AuthenticationInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var userIdHeader = context.RequestHeaders.FirstOrDefault(h => h.Key == "user_id");

        if (userIdHeader == null || string.IsNullOrEmpty(userIdHeader.Value))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Header 'user_id' is required."));
        }

        return await continuation(request, context);
    }
}