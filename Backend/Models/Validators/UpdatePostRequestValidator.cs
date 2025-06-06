using FluentValidation;

namespace Forum.Models.Validators;

public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(1).WithMessage("Title must be at least 1 character.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MinimumLength(1).WithMessage("Content must be at least 1 character.")
            .MaximumLength(5000).WithMessage("Content must not exceed 5000 characters.");
    }
}