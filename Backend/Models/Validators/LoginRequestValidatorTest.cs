using FluentValidation.TestHelper;
using Forum.Models;
using Forum.Models.Validators;
using Xunit;

namespace Forum.ValidatorTests;

public class LoginRequestValidatorTest
{
    private readonly LoginRequestValidator _validator;

    public LoginRequestValidatorTest()
    {
        _validator = new LoginRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_UsernameOrEmail_Is_Empty()
    {
        var model = new LoginRequest("", "password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UsernameOrEmail);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var model = new LoginRequest("testuser", "");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new LoginRequest("invalid-email@", "password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UsernameOrEmail);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Username_Is_Valid()
    {
        var model = new LoginRequest("testuser", "password123");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.UsernameOrEmail);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Email_Is_Valid()
    {
        var model = new LoginRequest("foobar", "password123");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.UsernameOrEmail);
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Short()
    {
        var model = new LoginRequest("ab", "password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UsernameOrEmail);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var model = new LoginRequest("testuser", "123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("testuser")]
    [InlineData("foobar@example.com")]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid(string usernameOrEmail)
    {
        var model = new LoginRequest(usernameOrEmail, "password123");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}