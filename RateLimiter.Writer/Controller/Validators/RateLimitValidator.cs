using System.Text.RegularExpressions;
using FluentValidation.Results;
using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Controller.Validators;

using FluentValidation;

public class RateLimitValidator : AbstractValidator<RateLimitDomainModel>
{
    private readonly Regex _routePattern = new Regex(@"^(((http|https|grpc|ws|wss)://)?(localhost|\d{1,3}(\.\d{1,3}){3}):\d{2,5})?/?(\w+\.\w+)/(Create|Get|Update|Delete)\w*$", RegexOptions.IgnoreCase);
    public RateLimitValidator()
    {
        RuleFor(rateLimit => rateLimit.Route)
            .NotEmpty().WithMessage("Route cannot be empty.")
            .Matches(_routePattern).WithMessage("Invalid route format.");
        RuleFor(rateLimit => rateLimit.RequestsPerMinute)
            .GreaterThan(0).WithMessage("Request per minute must be greater than zero.");
    }

    public ValidationResult ValidateOnlyRoute(string route)
    {
        var result = new ValidationResult();
    
        if (string.IsNullOrEmpty(route))
        {
            result.Errors.Add(new ValidationFailure("Route", "Route cannot be empty."));
        }
        else if (!_routePattern.IsMatch(route))
        {
            result.Errors.Add(new ValidationFailure("Route", "Invalid route format."));
        }
    
        return result;
    }
}