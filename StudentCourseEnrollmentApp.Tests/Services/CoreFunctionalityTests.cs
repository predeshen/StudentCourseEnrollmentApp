using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Domain;
using StudentCourseEnrollmentApp.Infrastructure.Data;
using StudentCourseEnrollmentApp.Infrastructure.Data.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Tests.Services
{
    [TestFixture]
    public class CoreFunctionalityTests
    {
        private ApplicationDbContext _context;
        private EnrollmentService _enrollmentService;
        private CourseService _courseService;
        private ApplicationUser _testUser;
        private Course _testCourse1;
        private Course _testCourse2;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            
            // Create test user
            _testUser = new ApplicationUser
            {
                Id = "test-student-123",
                UserName = "student@test.com",
                Email = "student@test.com",
                FirstName = "Test",
                LastName = "Student",
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            // Create test courses
            _testCourse1 = new Course
            {
                CourseId = 1,
                CourseTitle = "Introduction to Programming",
                CourseCode = "PROG101",
                Description = "Learn the basics of programming",
                Credits = 3
            };

            _testCourse2 = new Course
            {
                CourseId = 2,
                CourseTitle = "Advanced Mathematics",
                CourseCode = "MATH201",
                Description = "Advanced mathematical concepts",
                Credits = 4
            };

            _context.Users.Add(_testUser);
            _context.Courses.Add(_testCourse1);
            _context.Courses.Add(_testCourse2);
            _context.SaveChanges();

            _enrollmentService = new EnrollmentService(_context);
            _courseService = new CourseService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Student_CanEnrollInMultipleCourses()
        {
            // Arrange
            var userId = _testUser.Id;

            // Act: Enroll in first course
            var enrollment1Result = await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);
            Assert.That(enrollment1Result, Is.True, "First enrollment should succeed");

            // Act: Enroll in second course
            var enrollment2Result = await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse2.CourseId);
            Assert.That(enrollment2Result, Is.True, "Second enrollment should succeed");

            // Assert: Student should see both enrollments
            var enrolledCourses = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(enrolledCourses.Count(), Is.EqualTo(2), "Student should see 2 enrolled courses");

            var courseIds = enrolledCourses.Select(c => c.CourseId).ToList();
            Assert.That(courseIds, Does.Contain(_testCourse1.CourseId), "Student should see first course");
            Assert.That(courseIds, Does.Contain(_testCourse2.CourseId), "Student should see second course");
        }

        [Test]
        public async Task Student_CanViewEnrolledCourses()
        {
            // Arrange
            var userId = _testUser.Id;
            await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);

            // Act
            var enrolledCourses = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);

            // Assert
            Assert.That(enrolledCourses.Count(), Is.EqualTo(1), "Student should see 1 enrolled course");
            var enrolledCourse = enrolledCourses.First();
            Assert.That(enrolledCourse.CourseId, Is.EqualTo(_testCourse1.CourseId), "Should show correct course ID");
            Assert.That(enrolledCourse.CourseTitle, Is.EqualTo(_testCourse1.CourseTitle), "Should show correct course title");
            Assert.That(enrolledCourse.CourseCode, Is.EqualTo(_testCourse1.CourseCode), "Should show correct course code");
            Assert.That(enrolledCourse.Description, Is.EqualTo(_testCourse1.Description), "Should show correct course description");
            Assert.That(enrolledCourse.Credits, Is.EqualTo(_testCourse1.Credits), "Should show correct course credits");
        }

        [Test]
        public async Task Student_CanViewAvailableCourses()
        {
            // Arrange
            var userId = _testUser.Id;
            await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);

            // Act
            var availableCourses = await _enrollmentService.GetAvailableCoursesByStudentIdAsync(userId);

            // Assert
            Assert.That(availableCourses.Count(), Is.EqualTo(1), "Student should see 1 available course");
            var availableCourse = availableCourses.First();
            Assert.That(availableCourse.CourseId, Is.EqualTo(_testCourse2.CourseId), "Should show correct available course");
            Assert.That(availableCourses.Any(c => c.CourseId == _testCourse1.CourseId), Is.False, "Enrolled course should not be available");
        }

        [Test]
        public async Task Student_CanDeregisterFromCourse()
        {
            // Arrange
            var userId = _testUser.Id;
            await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);

            // Verify enrollment exists
            var enrolledCourses = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(enrolledCourses.Count(), Is.EqualTo(1), "Student should see 1 enrollment before deregistration");

            // Act: Deregister from course
            var deregistrationResult = await _enrollmentService.DeregisterStudentFromCourseAsync(userId, _testCourse1.CourseId);

            // Assert: Deregistration should succeed
            Assert.That(deregistrationResult, Is.True, "Deregistration should succeed");

            // Verify enrollment no longer exists
            var enrolledCoursesAfter = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(enrolledCoursesAfter.Count(), Is.EqualTo(0), "Student should see 0 enrollments after deregistration");

            // Verify course is now available again
            var availableCourses = await _enrollmentService.GetAvailableCoursesByStudentIdAsync(userId);
            Assert.That(availableCourses.Count(), Is.EqualTo(2), "Student should see 2 available courses after deregistration");
            Assert.That(availableCourses.Any(c => c.CourseId == _testCourse1.CourseId), Is.True, "Deregistered course should be available again");
        }

        [Test]
        public async Task Student_CannotEnrollInSameCourseTwice()
        {
            // Arrange
            var userId = _testUser.Id;

            // Act: First enrollment
            var firstEnrollment = await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);
            Assert.That(firstEnrollment, Is.True, "First enrollment should succeed");

            // Act: Second enrollment attempt
            var secondEnrollment = await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);

            // Assert: Second enrollment should fail
            Assert.That(secondEnrollment, Is.False, "Second enrollment should fail - already enrolled");

            // Verify only one enrollment exists
            var enrolledCourses = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(enrolledCourses.Count(), Is.EqualTo(1), "Student should still see only 1 enrollment");
        }

        [Test]
        public async Task Student_CannotDeregisterFromNonEnrolledCourse()
        {
            // Arrange
            var userId = _testUser.Id;
            // Don't enroll in any course

            // Act: Try to deregister from non-enrolled course
            var deregistrationResult = await _enrollmentService.DeregisterStudentFromCourseAsync(userId, _testCourse1.CourseId);

            // Assert: Deregistration should fail
            Assert.That(deregistrationResult, Is.False, "Deregistration should fail - not enrolled");

            // Verify no enrollments exist
            var enrolledCourses = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(enrolledCourses.Count(), Is.EqualTo(0), "Student should see 0 enrollments");
        }

        [Test]
        public async Task AvailableCourses_ShouldExcludeAllEnrolledCourses()
        {
            // Arrange
            var userId = _testUser.Id;
            await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);
            await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse2.CourseId);

            // Act
            var availableCourses = await _enrollmentService.GetAvailableCoursesByStudentIdAsync(userId);

            // Assert: No courses should be available since student is enrolled in all
            Assert.That(availableCourses.Count(), Is.EqualTo(0), "Student should see 0 available courses when enrolled in all");

            // Verify specific courses are not available
            Assert.That(availableCourses.Any(c => c.CourseId == _testCourse1.CourseId), Is.False, "First course should not be available");
            Assert.That(availableCourses.Any(c => c.CourseId == _testCourse2.CourseId), Is.False, "Second course should not be available");
        }

        [Test]
        public async Task CourseService_CanGetAllCourses()
        {
            // Act
            var allCourses = await _courseService.GetAllCoursesAsync();

            // Assert
            Assert.That(allCourses.Count(), Is.EqualTo(2), "Should return 2 courses");
            
            var courseIds = allCourses.Select(c => c.CourseId).ToList();
            Assert.That(courseIds, Does.Contain(_testCourse1.CourseId), "Should include first course");
            Assert.That(courseIds, Does.Contain(_testCourse2.CourseId), "Should include second course");
        }

        [Test]
        public async Task CourseService_CanGetCourseById()
        {
            // Act
            var course = await _courseService.GetCourseByIdAsync(_testCourse1.CourseId);

            // Assert
            Assert.That(course, Is.Not.Null, "Course should not be null");
            Assert.That(course.CourseId, Is.EqualTo(_testCourse1.CourseId), "Should return correct course ID");
            Assert.That(course.CourseTitle, Is.EqualTo(_testCourse1.CourseTitle), "Should return correct course title");
            Assert.That(course.CourseCode, Is.EqualTo(_testCourse1.CourseCode), "Should return correct course code");
            Assert.That(course.Description, Is.EqualTo(_testCourse1.Description), "Should return correct course description");
            Assert.That(course.Credits, Is.EqualTo(_testCourse1.Credits), "Should return correct course credits");
        }

        [Test]
        public async Task CourseService_GetCourseById_WhenNotFound_ShouldReturnNull()
        {
            // Act
            var course = await _courseService.GetCourseByIdAsync(999); // Non-existent course ID

            // Assert
            Assert.That(course, Is.Null, "Should return null for non-existent course");
        }

        [Test]
        public async Task Enrollment_ShouldHaveCorrectEnrollmentDate()
        {
            // Arrange
            var userId = _testUser.Id;
            var beforeEnrollment = DateTime.UtcNow;

            // Act
            await _enrollmentService.EnrollStudentInCourseAsync(userId, _testCourse1.CourseId);

            // Get the enrollment from context to check the date
            var enrollment = _context.Enrollments
                .FirstOrDefault(e => e.UserId == userId && e.CourseId == _testCourse1.CourseId);

            // Assert
            Assert.That(enrollment, Is.Not.Null, "Enrollment should exist");
            Assert.That(enrollment.EnrollmentDate >= beforeEnrollment, Is.True, "Enrollment date should be after or equal to before enrollment time");
            Assert.That(enrollment.EnrollmentDate <= DateTime.UtcNow, Is.True, "Enrollment date should be before or equal to current time");
        }

        [Test]
        public async Task MultipleStudents_CanEnrollInSameCourse()
        {
            // Arrange
            var student1 = new ApplicationUser
            {
                Id = "student-1",
                UserName = "student1@test.com",
                Email = "student1@test.com",
                FirstName = "Student",
                LastName = "One",
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            var student2 = new ApplicationUser
            {
                Id = "student-2",
                UserName = "student2@test.com",
                Email = "student2@test.com",
                FirstName = "Student",
                LastName = "Two",
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(student1);
            _context.Users.Add(student2);
            await _context.SaveChangesAsync();

            // Act: Both students enroll in the same course
            var enrollment1Result = await _enrollmentService.EnrollStudentInCourseAsync(student1.Id, _testCourse1.CourseId);
            var enrollment2Result = await _enrollmentService.EnrollStudentInCourseAsync(student2.Id, _testCourse1.CourseId);

            // Assert: Both enrollments should succeed
            Assert.That(enrollment1Result, Is.True, "First student enrollment should succeed");
            Assert.That(enrollment2Result, Is.True, "Second student enrollment should succeed");

            // Verify both students see the course as enrolled
            var student1Enrollments = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(student1.Id);
            var student2Enrollments = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(student2.Id);

            Assert.That(student1Enrollments.Count(), Is.EqualTo(1), "First student should see 1 enrollment");
            Assert.That(student2Enrollments.Count(), Is.EqualTo(1), "Second student should see 1 enrollment");

            Assert.That(student1Enrollments.First().CourseId, Is.EqualTo(_testCourse1.CourseId), "First student should see correct course");
            Assert.That(student2Enrollments.First().CourseId, Is.EqualTo(_testCourse1.CourseId), "Second student should see correct course");
        }
    }
}
