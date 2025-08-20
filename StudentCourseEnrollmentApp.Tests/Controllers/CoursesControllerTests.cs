using Moq;
using NUnit.Framework;
using StudentCourseEnrollmentApp.API.Controllers;
using StudentCourseEnrollmentApp.Core.Application.Services;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[TestFixture]
public class CoursesControllerTests
{
    private Mock<ICourseService> _mockCourseService;
    private CoursesController _controller;

    [SetUp]
    public void Setup()
    {
        _mockCourseService = new Mock<ICourseService>();
        _controller = new CoursesController(_mockCourseService.Object);
    }

    [Test]
    public async Task GetAllCourses_ShouldReturnOkWithCourses()
    {
        // Arrange
        var courses = new List<CourseDTO> 
        { 
            new CourseDTO { CourseId = 1, CourseTitle = "Test Course 1", CourseCode = "TC101", Description = "Test Description 1", Credits = 3 },
            new CourseDTO { CourseId = 2, CourseTitle = "Test Course 2", CourseCode = "TC102", Description = "Test Description 2", Credits = 4 }
        };
        _mockCourseService.Setup(s => s.GetAllCoursesAsync())
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.GetAllCourses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(courses));
    }

    [Test]
    public async Task GetAllCourses_ShouldReturnEmptyList_WhenNoCoursesExist()
    {
        // Arrange
        var courses = new List<CourseDTO>();
        _mockCourseService.Setup(s => s.GetAllCoursesAsync())
            .ReturnsAsync(courses);

        // Act
        var result = await _controller.GetAllCourses();

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(courses));
    }

    [Test]
    public async Task GetCourseById_ShouldReturnOkWithCourse_WhenCourseExists()
    {
        // Arrange
        var course = new CourseDTO { CourseId = 1, CourseTitle = "Test Course", CourseCode = "TC101", Description = "Test Description", Credits = 3 };
        _mockCourseService.Setup(s => s.GetCourseByIdAsync(1))
            .ReturnsAsync(course);

        // Act
        var result = await _controller.GetCourseById(1);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(course));
    }

    [Test]
    public async Task GetCourseById_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        _mockCourseService.Setup(s => s.GetCourseByIdAsync(999))
            .ReturnsAsync((CourseDTO)null);

        // Act
        var result = await _controller.GetCourseById(999);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }
}
