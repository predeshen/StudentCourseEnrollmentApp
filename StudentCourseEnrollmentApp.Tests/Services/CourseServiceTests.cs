using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Infrastructure.Data;
using StudentCourseEnrollmentApp.Infrastructure.Data.Services;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

[TestFixture]
public class CourseServiceTests
{
    private ApplicationDbContext _context;
    private CourseService _courseService;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Test_CourseDb")
            .Options;
        _context = new ApplicationDbContext(options);

        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _courseService = new CourseService(_context);
    }

    [Test]
    public async Task GetAllCoursesAsync_ShouldReturnAllCourses()
    {
        // Arrange
        _context.Courses.Add(new StudentCourseEnrollmentApp.Core.Domain.Course { CourseId = 5, CourseCode = "C101", CourseTitle = "Test 1" });
        _context.Courses.Add(new StudentCourseEnrollmentApp.Core.Domain.Course { CourseId = 6, CourseCode = "C102", CourseTitle = "Test 2" });
        await _context.SaveChangesAsync();

        // Act
        var courses = await _courseService.GetAllCoursesAsync();

        // Assert
        Assert.That(courses.Count(), Is.EqualTo(5));
        Assert.That(courses.Any(c => c.CourseTitle == "Test 1"), Is.True);
    }

    [Test]
    public async Task GetCourseByIdAsync_ShouldReturnCorrectCourse()
    {
        // Arrange
        var course = new StudentCourseEnrollmentApp.Core.Domain.Course { CourseId = 4, CourseCode = "C103", CourseTitle = "Find Me" };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var foundCourse = await _courseService.GetCourseByIdAsync(4);

        // Assert
        Assert.That(foundCourse, Is.Not.Null);
        Assert.That(foundCourse.CourseTitle, Is.EqualTo("Find Me"));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}