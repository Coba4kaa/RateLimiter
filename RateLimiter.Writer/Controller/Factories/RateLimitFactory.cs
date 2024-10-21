using RateLimiter.Writer.Grpc;
using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Controller.Factories;

public static class RateLimitFactory
{
    public static RateLimitDomainModel CreateRateLimitDomainModel(CreateRateLimitRequest request)
    {
        return new RateLimitDomainModel(request.RateLimit.Route, request.RateLimit.RequestsPerMinute);
    }
    
    public static RateLimitDomainModel CreateRateLimitDomainModel(UpdateRateLimitRequest request)
    {
        return new RateLimitDomainModel(request.RateLimit.Route, request.RateLimit.RequestsPerMinute);
    }
}