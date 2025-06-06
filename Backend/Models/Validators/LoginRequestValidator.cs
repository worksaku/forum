using FluentValidation;

namespace Forum.Models.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        When(x => x.UsernameOrEmail.Contains('@'), () =>
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        })
        .Otherwise(() =>
        {
            RuleFor(x => x.UsernameOrEmail)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters.");
        });

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}