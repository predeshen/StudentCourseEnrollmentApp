using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StudentCourseEnrollmentApp.API.Controllers;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Tests.Controllers
{
    [TestFixture]
    public class AdminControllerTests
    {
        private AdminController _controller;
        private Mock<IAdminService> _mockAdminService;
        private List<Claim> _claims;

        [SetUp]
        public void Setup()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminController(_mockAdminService.Object);
            
            // Setup super admin claims
            _claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-user-123"),
                new Claim(ClaimTypes.Email, "admin@admin.com"),
                new Claim(ClaimTypes.Name, "admin@admin.com"),
                new Claim("IsSuperAdmin", "True")
            };
        }

        private void SetUserContext(bool isSuperAdmin = true)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", "admin-user-123"),
                new Claim(ClaimTypes.Email, "admin@admin.com"),
                new Claim(ClaimTypes.Name, "admin@admin.com")
            };

            if (isSuperAdmin)
            {
                claims.Add(new Claim("IsSuperAdmin", "True"));
            }

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims))
                }
            };
        }

        [Test]
        public async Task CreateUser_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var createUserDto = new CreateUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                Password = "Password123!",
                IsSuperAdmin = false
            };

            var expectedUser = new ApplicationUserDTO
            {
                Id = "user-123",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminService.Setup(x => x.CreateUserAsync(createUserDto))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtResult = result as CreatedAtActionResult;
            Assert.That(createdAtResult.Value, Is.EqualTo(expectedUser));
            Assert.That(createdAtResult.ActionName, Is.EqualTo("GetUserById"));
            Assert.That(createdAtResult.RouteValues.Keys.First(), Is.EqualTo("userId"));
            Assert.That(createdAtResult.RouteValues["userId"], Is.EqualTo(expectedUser.Id));
        }

        [Test]
        public async Task CreateUser_WithoutSuperAdmin_ShouldReturnForbid()
        {
            // Arrange
            SetUserContext(false);
            var createUserDto = new CreateUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                Password = "Password123!",
                IsSuperAdmin = false
            };

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task CreateUser_WithServiceException_ShouldReturnBadRequest()
        {
            // Arrange
            SetUserContext(true);
            var createUserDto = new CreateUserDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@test.com",
                Password = "Password123!",
                IsSuperAdmin = false
            };

            _mockAdminService.Setup(x => x.CreateUserAsync(createUserDto))
                .ThrowsAsync(new InvalidOperationException("User creation failed"));

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("User creation failed"));
        }

        [Test]
        public async Task CreateCourse_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var createCourseDto = new CreateCourseDTO
            {
                CourseTitle = "Test Course",
                CourseCode = "TEST101",
                Description = "A test course",
                Credits = 3
            };

            var expectedCourse = new CourseDTO
            {
                CourseId = 1,
                CourseTitle = "Test Course",
                CourseCode = "TEST101",
                Description = "A test course",
                Credits = 3
            };

            _mockAdminService.Setup(x => x.CreateCourseAsync(createCourseDto))
                .ReturnsAsync(expectedCourse);

            // Act
            var result = await _controller.CreateCourse(createCourseDto);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdAtResult = result as CreatedAtActionResult;
            Assert.That(createdAtResult.Value, Is.EqualTo(expectedCourse));
        }

        [Test]
        public async Task CreateCourse_WithoutSuperAdmin_ShouldReturnForbid()
        {
            // Arrange
            SetUserContext(false);
            var createCourseDto = new CreateCourseDTO
            {
                CourseTitle = "Test Course",
                CourseCode = "TEST101",
                Description = "A test course",
                Credits = 3
            };

            // Act
            var result = await _controller.CreateCourse(createCourseDto);

            // Assert
            Assert.That(result, Is.InstanceOf<ForbidResult>());
        }

        [Test]
        public async Task EnrollUserInCourse_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var enrollmentRequest = new AdminEnrollmentRequestDTO
            {
                UserId = "user-123",
                CourseId = 1
            };

            _mockAdminService.Setup(x => x.EnrollUserInCourseAsync(enrollmentRequest.UserId, enrollmentRequest.CourseId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.EnrollUserInCourse(enrollmentRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("User enrolled successfully"));
        }

        [Test]
        public async Task EnrollUserInCourse_WithSuperAdmin_WhenAlreadyEnrolled_ShouldReturnBadRequest()
        {
            // Arrange
            SetUserContext(true);
            var enrollmentRequest = new AdminEnrollmentRequestDTO
            {
                UserId = "user-123",
                CourseId = 1
            };

            _mockAdminService.Setup(x => x.EnrollUserInCourseAsync(enrollmentRequest.UserId, enrollmentRequest.CourseId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.EnrollUserInCourse(enrollmentRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("User is already enrolled in this course or invalid request"));
        }

        [Test]
        public async Task DeregisterUserFromCourse_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var enrollmentRequest = new AdminEnrollmentRequestDTO
            {
                UserId = "user-123",
                CourseId = 1
            };

            _mockAdminService.Setup(x => x.DeregisterUserFromCourseAsync(enrollmentRequest.UserId, enrollmentRequest.CourseId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeregisterUserFromCourse(enrollmentRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("User deregistered successfully"));
        }

        [Test]
        public async Task DeregisterUserFromCourse_WithSuperAdmin_WhenEnrollmentNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            SetUserContext(true);
            var enrollmentRequest = new AdminEnrollmentRequestDTO
            {
                UserId = "user-123",
                CourseId = 1
            };

            _mockAdminService.Setup(x => x.DeregisterUserFromCourseAsync(enrollmentRequest.UserId, enrollmentRequest.CourseId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeregisterUserFromCourse(enrollmentRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Enrollment not found or invalid request"));
        }

        [Test]
        public async Task GetCourseEnrollmentSummary_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var courseId = 1;
            var expectedSummary = new CourseEnrollmentSummaryDTO
            {
                CourseId = courseId,
                CourseTitle = "Test Course",
                CourseCode = "TEST101",
                TotalEnrollments = 2,
                EnrolledUsers = new List<UserEnrollmentDTO>
                {
                    new UserEnrollmentDTO
                    {
                        UserId = "user-1",
                        Email = "user1@test.com",
                        FirstName = "User",
                        LastName = "One",
                        EnrollmentDate = DateTime.UtcNow,
                        IsSuperAdmin = false
                    },
                    new UserEnrollmentDTO
                    {
                        UserId = "user-2",
                        Email = "user2@test.com",
                        FirstName = "User",
                        LastName = "Two",
                        EnrollmentDate = DateTime.UtcNow,
                        IsSuperAdmin = false
                    }
                }
            };

            _mockAdminService.Setup(x => x.GetCourseEnrollmentSummaryAsync(courseId))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _controller.GetCourseEnrollmentSummary(courseId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(expectedSummary));
        }

        [Test]
        public async Task GetCourseEnrollmentSummary_WithSuperAdmin_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            SetUserContext(true);
            var courseId = 1;

            _mockAdminService.Setup(x => x.GetCourseEnrollmentSummaryAsync(courseId))
                .ThrowsAsync(new InvalidOperationException("Course not found"));

            // Act
            var result = await _controller.GetCourseEnrollmentSummary(courseId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Course not found"));
        }

        [Test]
        public async Task GetAllEnrollments_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var expectedEnrollments = new List<AdminEnrollmentDTO>
            {
                new AdminEnrollmentDTO
                {
                    UserId = "user-1",
                    CourseId = 1,
                    UserEmail = "user1@test.com",
                    UserName = "User One",
                    CourseTitle = "Test Course 1",
                    EnrollmentDate = DateTime.UtcNow
                },
                new AdminEnrollmentDTO
                {
                    UserId = "user-2",
                    CourseId = 2,
                    UserEmail = "user2@test.com",
                    UserName = "User Two",
                    CourseTitle = "Test Course 2",
                    EnrollmentDate = DateTime.UtcNow
                }
            };

            _mockAdminService.Setup(x => x.GetAllEnrollmentsAsync())
                .ReturnsAsync(expectedEnrollments);

            // Act
            var result = await _controller.GetAllEnrollments();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(expectedEnrollments));
        }

        [Test]
        public async Task GetAllUsers_WithSuperAdmin_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var expectedUsers = new List<ApplicationUserDTO>
            {
                new ApplicationUserDTO
                {
                    Id = "user-1",
                    Email = "user1@test.com",
                    FirstName = "User",
                    LastName = "One",
                    IsSuperAdmin = false,
                    CreatedAt = DateTime.UtcNow
                },
                new ApplicationUserDTO
                {
                    Id = "user-2",
                    Email = "user2@test.com",
                    FirstName = "User",
                    LastName = "Two",
                    IsSuperAdmin = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockAdminService.Setup(x => x.GetAllUsersAsync())
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(expectedUsers));
        }

        [Test]
        public async Task GetUserById_WithSuperAdmin_WhenUserExists_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var userId = "user-123";
            var expectedUser = new ApplicationUserDTO
            {
                Id = userId,
                Email = "user@test.com",
                FirstName = "Test",
                LastName = "User",
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockAdminService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(expectedUser));
        }

        [Test]
        public async Task GetUserById_WithSuperAdmin_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            SetUserContext(true);
            var userId = "nonexistent-user";

            _mockAdminService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((ApplicationUserDTO)null);

            // Act
            var result = await _controller.GetUserById(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteUser_WithSuperAdmin_WhenUserExists_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var userId = "user-123";

            _mockAdminService.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("User deleted successfully"));
        }

        [Test]
        public async Task DeleteUser_WithSuperAdmin_WhenUserCannotBeDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            SetUserContext(true);
            var userId = "user-123";

            _mockAdminService.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Cannot delete user with enrollments or user not found"));
        }

        [Test]
        public async Task DeleteCourse_WithSuperAdmin_WhenCourseExists_ShouldSucceed()
        {
            // Arrange
            SetUserContext(true);
            var courseId = 1;

            _mockAdminService.Setup(x => x.DeleteCourseAsync(courseId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCourse(courseId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("Course deleted successfully"));
        }

        [Test]
        public async Task DeleteCourse_WithSuperAdmin_WhenCourseCannotBeDeleted_ShouldReturnBadRequest()
        {
            // Arrange
            SetUserContext(true);
            var courseId = 1;

            _mockAdminService.Setup(x => x.DeleteCourseAsync(courseId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCourse(courseId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult.Value, Is.EqualTo("Cannot delete course with enrollments or course not found"));
        }
    }
}
