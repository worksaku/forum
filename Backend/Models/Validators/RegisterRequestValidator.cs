using FluentValidation;

namespace Forum.Models.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(20).WithMessage("Username must not exceed 20 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$").WithMessage("Password must contain at least one uppercase letter, one lowercase letter, and one number.");
    }
}