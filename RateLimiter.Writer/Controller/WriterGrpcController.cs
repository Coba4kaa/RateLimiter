using Grpc.Core;
using RateLimiter.Writer.Controller.Factories;
using RateLimiter.Writer.Service.DomainServices;
using RateLimiter.Writer.Grpc;
using RateLimiter.Writer.Controller.Validators;

namespace RateLimiter.Writer.Controller;

public class WriterGrpcController : Grpc.Writer.WriterBase
{
    private readonly IWriterService _writerService;
    private readonly RateLimitValidator _rateLimitValidator;

    public WriterGrpcController(IWriterService writerService, RateLimitValidator rateLimitValidator)
    {
        _writerService = writerService;
        _rateLimitValidator = rateLimitValidator;
    }
    
    public override async Task<CreateRateLimitResponse> CreateRateLimit(CreateRateLimitRequest request,
        ServerCallContext context)
    {
        var rateLimit = RateLimitFactory.CreateRateLimitDomainModel(request);
        var validationResult = _rateLimitValidator.Validate(rateLimit);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }
        var cancellationToken = context.CancellationToken;
        var success = await _writerService.CreateRateLimit(rateLimit, cancellationToken);
        return new CreateRateLimitResponse
        {
            Success = success,
            StatusCode = success ? (int)StatusCode.OK : (int)StatusCode.InvalidArgument
        };
    }

    public override async Task<GetRateLimitByRouteResponse> GetRateLimitByRoute(GetRateLimitByRouteRequest request,
        ServerCallContext context)
    {
        var validationResult = _rateLimitValidator.ValidateOnlyRoute(request.Route);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }
        var cancellationToken = context.CancellationToken;
        var foundRateLimit = await _writerService.GetRateLimitByRoute(request.Route, cancellationToken);
        if (foundRateLimit == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Rate limit by route {request.Route} does not exist."));
        }
        return new GetRateLimitByRouteResponse
        {
            RateLimit = new RateLimit 
            { 
                Route = foundRateLimit.Route, 
                RequestsPerMinute = foundRateLimit.RequestsPerMinute 
            }
        };
    }

    public override async Task<UpdateRateLimitResponse> UpdateRateLimit(UpdateRateLimitRequest request,
        ServerCallContext context)
    {
        var rateLimit = RateLimitFactory.CreateRateLimitDomainModel(request);
        var validationResult = _rateLimitValidator.Validate(rateLimit);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }
        var cancellationToken = context.CancellationToken;
        var success = await _writerService.UpdateRateLimit(rateLimit, cancellationToken);
        return new UpdateRateLimitResponse
        {
            Success = success,
            StatusCode = success ? (int)StatusCode.OK : (int)StatusCode.InvalidArgument
        };
    }

    public override async Task<DeleteRateLimitResponse> DeleteRateLimit(DeleteRateLimitRequest request,
        ServerCallContext context)
    {
        var validationResult = _rateLimitValidator.ValidateOnlyRoute(request.Route);
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new RpcException(new Status(StatusCode.InvalidArgument, errors));
        }
        var cancellationToken = context.CancellationToken;
        var success = await _writerService.DeleteRateLimit(request.Route, cancellationToken);
        return new DeleteRateLimitResponse
        {
            Success = success,
            StatusCode = success ? (int)StatusCode.OK : (int)StatusCode.InvalidArgument
        };
    }
}