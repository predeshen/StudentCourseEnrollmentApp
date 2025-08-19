using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Domain;
using StudentCourseEnrollmentApp.Infrastructure.Data;
using StudentCourseEnrollmentApp.Infrastructure.Data.Services;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

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
        var student = new Student { StudentId = 1, Email = "test@test.com" };
        var course = new Course { CourseId = 101, CourseTitle = "Intro" };
        _context.Students.Add(student);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.EnrollStudentInCourseAsync(student.StudentId, course.CourseId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_context.Enrollments.Any(e => e.StudentId == student.StudentId && e.CourseId == course.CourseId), Is.True);
    }

    [Test]
    public async Task DeregisterStudentFromCourseAsync_ShouldReturnTrueOnSuccess()
    {
        // Arrange
        var student = new Student { StudentId = 2, Email = "test2@test.com" };
        var course = new Course { CourseId = 102, CourseTitle = "Advanced" };
        var enrollment = new Enrollment { StudentId = student.StudentId, CourseId = course.CourseId };
        _context.Students.Add(student);
        _context.Courses.Add(course);
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        // Act
        var result = await _enrollmentService.DeregisterStudentFromCourseAsync(student.StudentId, course.CourseId);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_context.Enrollments.Any(e => e.StudentId == student.StudentId && e.CourseId == course.CourseId), Is.False);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}