using FluentValidation.TestHelper;
using Forum.Models.Validators;
using Xunit;
using Forum.Models;

namespace Forum.ValidatorTests;

public class RegisterRequestValidatorTest
{
    private readonly RegisterRequestValidator _validator;

    public RegisterRequestValidatorTest()
    {
        _validator = new RegisterRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var model = new RegisterRequest("", "example@example.com", "Password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var model = new RegisterRequest("testuser", "", "Password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var model = new RegisterRequest("testuser", "example@example.com", "");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Short()
    {
        var model = new RegisterRequest("ab", "example@example.com", "Password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Long()
    {
        var model = new RegisterRequest(new string('a', 21), "example@example.com", "Password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var model = new RegisterRequest("testuser", "invalid-email@", "Password123");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var model = new RegisterRequest("testuser", "example@example.com", "short");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Does_Not_Contain_Required_Characters()
    {
        var model = new RegisterRequest("testuser", "example@example.com", "password");
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
    {
        var model = new RegisterRequest("validuser", "example@example.com", "Valid123!");
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}