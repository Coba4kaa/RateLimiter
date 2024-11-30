using UserService.Service.DomainInterface;

namespace UserService.Controller;

using FluentValidation;

public class UserValidator : AbstractValidator<IUser>
{
    public UserValidator()
    {
        RuleFor(user => user.Login)
            .NotEmpty().WithMessage("Login must not be empty.")
            .Length(4, 20).WithMessage("Login must be between 4 and 20 characters.")
            .When(user => user is CreateUserRequestModel);
        
        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password must not be empty.")
            .MinimumLength(4).WithMessage("Password must be at least 4 characters.");

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name must not be empty.");

        RuleFor(user => user.Surname)
            .NotEmpty().WithMessage("Surname must not be empty.");

        RuleFor(user => user.Age)
            .GreaterThan(0).WithMessage("Age must be greater than 0.");
    }
}
