using FluentValidation.TestHelper;
using Forum.Models.Validators;
using Xunit;
using Forum.Models;

namespace Forum.ValidatorTests;

public class CreatePostRequestValidatorTest
{
    private readonly CreatePostRequestValidator _validator;

    public CreatePostRequestValidatorTest()
    {
        _validator = new CreatePostRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var model = new CreatePostRequest("", "This is the content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        var model = new CreatePostRequest("Post Title", "");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Too_Long()
    {
        var model = new CreatePostRequest(new string('a', 101), "This is the content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Too_Long()
    {
        var model = new CreatePostRequest("Post Title", new string('a', 5001));
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Title_And_Content_Are_Valid()
    {
        var model = new CreatePostRequest("Valid Post Title", "This is the content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Whitespace()
    {
        var model = new CreatePostRequest("   ", "This is the content of the post.");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Whitespace()
    {
        var model = new CreatePostRequest("Post Title", "   ");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Content);
    }
}