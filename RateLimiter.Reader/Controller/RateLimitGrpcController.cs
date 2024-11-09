using Grpc.Core;
using RateLimiter.Reader.Grpc;
using RateLimiter.Reader.Service.DomainServices;

namespace RateLimiter.Reader.Controller
{
    public class RateLimitGrpcController(IRateLimitService rateLimitService) : Grpc.RateLimitService.RateLimitServiceBase
    {
        public override Task<RateLimitResponse> GetRateLimits(RateLimitRequest request, ServerCallContext context)
        {
            var rateLimits = rateLimitService.GetCurrentRateLimits()
                .Select(limit => new RateLimit
                {
                    Route = limit.Route,
                    RequestsPerMinute = limit.RequestsPerMinute
                })
                .ToList();

            var response = new RateLimitResponse();
            response.RateLimits.AddRange(rateLimits);
            
            return Task.FromResult(response);
        }
    }
}