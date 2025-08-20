using Moq;
using NUnit.Framework;
using StudentCourseEnrollmentApp.API.Controllers;
using StudentCourseEnrollmentApp.Core.Application.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Collections.Generic;

[TestFixture]
public class EnrollmentsControllerTests
{
    private Mock<IEnrollmentService> _mockEnrollmentService;
    private EnrollmentsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockEnrollmentService = new Mock<IEnrollmentService>();
        _controller = new EnrollmentsController(_mockEnrollmentService.Object);

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Test]
    public async Task GetMyEnrolledCourses_ShouldReturnOkWithCourses()
    {
        // Arrange
        var courses = new List<CourseDTO> { new CourseDTO { CourseId = 1, CourseTitle = "Test Course" } };
        _mockEnrollmentService.Setup(s => s.GetEnrolledCoursesByStudentIdAsync("1"))
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.GetMyEnrolledCourses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(courses));
    }

    [Test]
    public async Task DeregisterFromCourse_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        int courseId = 1;
        _mockEnrollmentService.Setup(s => s.DeregisterStudentFromCourseAsync("1", courseId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeregisterFromCourse(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task DeregisterFromCourse_ShouldReturnNotFound_WhenEnrollmentDoesNotExist()
    {
        // Arrange
        int courseId = 99;
        _mockEnrollmentService.Setup(s => s.DeregisterStudentFromCourseAsync("1", courseId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeregisterFromCourse(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task EnrollInCourse_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        int courseId = 1;
        _mockEnrollmentService.Setup(s => s.EnrollStudentInCourseAsync("1", courseId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.EnrollInCourse(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo("Enrollment successful."));
    }

    [Test]
    public async Task EnrollInCourse_ShouldReturnBadRequest_WhenAlreadyEnrolled()
    {
        // Arrange
        int courseId = 1;
        _mockEnrollmentService.Setup(s => s.EnrollStudentInCourseAsync("1", courseId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.EnrollInCourse(courseId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("Enrollment failed. You may already be enrolled in this course."));
    }

    [Test]
    public async Task GetAvailableCourses_ShouldReturnOkWithCourses()
    {
        // Arrange
        var courses = new List<CourseDTO> { new CourseDTO { CourseId = 2, CourseTitle = "Available Course" } };
        _mockEnrollmentService.Setup(s => s.GetAvailableCoursesByStudentIdAsync("1"))
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.GetAvailableCourses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(courses));
    }

    [Test]
    public async Task GetMyEnrolledCourses_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        // Act
        var result = await _controller.GetMyEnrolledCourses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<UnauthorizedResult>());
    }
}