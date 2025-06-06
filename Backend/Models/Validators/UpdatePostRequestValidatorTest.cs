using FluentValidation.TestHelper;
using Forum.Models;
using Forum.Models.Validators;
using Xunit;

namespace Forum.ValidatorTests;

public class UpdatePostRequestValidatorTest
{
    private readonly UpdatePostRequestValidator _validator;

    public UpdatePostRequestValidatorTest()
    {
        _validator = new UpdatePostRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var model = new UpdatePostRequest("", "This is the updated content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        var model = new UpdatePostRequest("Updated Post Title", "");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Too_Long()
    {
        var model = new UpdatePostRequest(new string('a', 101), "This is the updated content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Too_Long()
    {
        var model = new UpdatePostRequest("Updated Post Title", new string('a', 5001));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_And_Content_Are_Valid()
    {
        var model = new UpdatePostRequest("Valid Updated Post Title", "This is the updated content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }
}