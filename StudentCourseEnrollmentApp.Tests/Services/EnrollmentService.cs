using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Domain;
using StudentCourseEnrollmentApp.Infrastructure.Data;
using StudentCourseEnrollmentApp.Infrastructure.Data.Services;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.AspNetCore.Identity;

[TestFixture]
public class EnrollmentServiceTests
{
    private ApplicationDbContext _context;
    private EnrollmentService _enrollmentService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_EnrollmentDb")
            .Options;
        _context = new ApplicationDbContext(options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _enrollmentService = new EnrollmentService(_context);
    }

    [Test]
    public async Task EnrollStudentInCourseAsync_ShouldReturnTrueOnSuccess()
    {
        // Arrange
        var userId = "test-user-id-123";
        var course = new Course { CourseId = 101, CourseTitle = "Intro" };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.EnrollStudentInCourseAsync(userId, course.CourseId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_context.Enrollments.Any(e => e.UserId == userId && e.CourseId == course.CourseId), Is.True);
    }

    [Test]
    public async Task EnrollStudentInCourseAsync_ShouldReturnFalse_WhenAlreadyEnrolled()
    {
        // Arrange
        var userId = "test-user-id-456";
        var course = new Course { CourseId = 102, CourseTitle = "Advanced" };
        var existingEnrollment = new Enrollment { UserId = userId, CourseId = course.CourseId };
        
        _context.Courses.Add(course);
        _context.Enrollments.Add(existingEnrollment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.EnrollStudentInCourseAsync(userId, course.CourseId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task DeregisterStudentFromCourseAsync_ShouldReturnTrueOnSuccess()
    {
        // Arrange
        var userId = "test-user-id-789";
        var course = new Course { CourseId = 103, CourseTitle = "Expert" };
        var enrollment = new Enrollment { UserId = userId, CourseId = course.CourseId };
        
        _context.Courses.Add(course);
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.DeregisterStudentFromCourseAsync(userId, course.CourseId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_context.Enrollments.Any(e => e.UserId == userId && e.CourseId == course.CourseId), Is.False);
    }

    [Test]
    public async Task DeregisterStudentFromCourseAsync_ShouldReturnFalse_WhenEnrollmentDoesNotExist()
    {
        // Arrange
        var userId = "test-user-id-999";
        var courseId = 999;

        // Act
        var result = await _enrollmentService.DeregisterStudentFromCourseAsync(userId, courseId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetEnrolledCoursesByStudentIdAsync_ShouldReturnEnrolledCourses()
    {
        // Arrange
        var userId = "test-user-id-111";
        var course1 = new Course { CourseId = 201, CourseTitle = "Math", CourseCode = "M101", Description = "Mathematics", Credits = 3 };
        var course2 = new Course { CourseId = 202, CourseTitle = "Science", CourseCode = "S101", Description = "Science", Credits = 4 };
        
        _context.Courses.AddRange(course1, course2);
        _context.Enrollments.AddRange(
            new Enrollment { UserId = userId, CourseId = course1.CourseId },
            new Enrollment { UserId = userId, CourseId = course2.CourseId }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(c => c.CourseId == course1.CourseId), Is.True);
        Assert.That(result.Any(c => c.CourseId == course2.CourseId), Is.True);
    }

    [Test]
    public async Task GetAvailableCoursesByStudentIdAsync_ShouldReturnNonEnrolledCourses()
    {
        // Arrange
        var userId = "test-user-id-222";
        var enrolledCourse = new Course { CourseId = 301, CourseTitle = "Enrolled Course", CourseCode = "E101", Description = "Already enrolled", Credits = 3 };
        var availableCourse = new Course { CourseId = 302, CourseTitle = "Available Course", CourseCode = "A101", Description = "Not enrolled", Credits = 4 };
        
        _context.Courses.AddRange(enrolledCourse, availableCourse);
        _context.Enrollments.Add(new Enrollment { UserId = userId, CourseId = enrolledCourse.CourseId });
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.GetAvailableCoursesByStudentIdAsync(userId);

        // Assert
        // Note: The result includes both the test courses and the seed data courses (minus the enrolled one)
        Assert.That(result.Count(), Is.GreaterThan(1));
        Assert.That(result.Any(c => c.CourseId == availableCourse.CourseId), Is.True);
        Assert.That(result.Any(c => c.CourseId == enrolledCourse.CourseId), Is.False);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}