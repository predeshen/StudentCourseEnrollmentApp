using Moq;
using NUnit.Framework;
using StudentCourseEnrollmentApp.API.Controllers;
using StudentCourseEnrollmentApp.Core.Application;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthenticationService> _mockAuthService;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthenticationService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Test]
    public async Task RegisterUser_ShouldReturnOk_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var registrationDto = new UserRegistrationRequestDTO { Email = "test@example.com", Password = "Password123" };
        var authResult = new AuthResultDTO { Result = true, Token = "fake-token" };
        _mockAuthService.Setup(s => s.RegisterUserAsync(It.IsAny<UserRegistrationRequestDTO>()))
                        .ReturnsAsync(authResult);

        // Act
        var result = await _controller.RegisterUser(registrationDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(authResult));
    }

    [Test]
    public async Task RegisterUser_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var registrationDto = new UserRegistrationRequestDTO { Email = "fail@example.com", Password = "weak" };
        var authResult = new AuthResultDTO { Result = false, Errors = new List<string> { "Weak password." } };
        _mockAuthService.Setup(s => s.RegisterUserAsync(It.IsAny<UserRegistrationRequestDTO>()))
                        .ReturnsAsync(authResult);

        // Act
        var result = await _controller.RegisterUser(registrationDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo(authResult.Errors));
    }

    [Test]
    public async Task LoginUser_ShouldReturnOk_WhenLoginIsSuccessful()
    {
        // Arrange
        var loginDto = new UserLoginRequestDTO { Email = "user@example.com", Password = "Password123" };
        var authResult = new AuthResultDTO { Result = true, Token = "another-fake-token" };
        _mockAuthService.Setup(s => s.LoginUserAsync(It.IsAny<UserLoginRequestDTO>()))
                        .ReturnsAsync(authResult);

        // Act
        var result = await _controller.LoginUser(loginDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(authResult));
    }
}