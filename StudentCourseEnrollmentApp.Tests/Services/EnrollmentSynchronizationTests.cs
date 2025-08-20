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
    public class EnrollmentSynchronizationTests
    {
        private ApplicationDbContext _context;
        private EnrollmentService _enrollmentService;
        private AdminService _adminService;
        private ApplicationUser _testUser;
        private Course _testCourse;

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
                Id = "test-user-123",
                UserName = "testuser@test.com",
                Email = "testuser@test.com",
                FirstName = "Test",
                LastName = "User",
                IsSuperAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            // Create test course
            _testCourse = new Course
            {
                CourseId = 1,
                CourseTitle = "Test Course",
                CourseCode = "TEST101",
                Description = "A test course",
                Credits = 3
            };

            _context.Users.Add(_testUser);
            _context.Courses.Add(_testCourse);
            _context.SaveChanges();

            _enrollmentService = new EnrollmentService(_context);
            _adminService = new AdminService(_context, null!); // We'll mock UserManager if needed
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task AdminEnrollment_ShouldBeVisibleToStudent()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Act: Admin enrolls user in course
            var adminEnrollmentResult = await _adminService.EnrollUserInCourseAsync(userId, courseId);

            // Assert: Admin enrollment was successful
            Assert.That(adminEnrollmentResult, Is.True, "Admin should be able to enroll user");

            // Act: Student tries to view their enrolled courses
            var studentEnrollments = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);

            // Assert: Student can see the admin-created enrollment
            Assert.That(studentEnrollments.Count(), Is.EqualTo(1), "Student should see 1 enrollment");
            Assert.That(studentEnrollments.First().CourseId, Is.EqualTo(courseId), "Student should see the correct course");
        }

        [Test]
        public async Task StudentEnrollment_ShouldBeVisibleToAdmin()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Act: Student enrolls themselves in course
            var studentEnrollmentResult = await _enrollmentService.EnrollStudentInCourseAsync(userId, courseId);

            // Assert: Student enrollment was successful
            Assert.That(studentEnrollmentResult, Is.True, "Student should be able to enroll themselves");

            // Act: Admin tries to view all enrollments
            var adminEnrollments = await _adminService.GetAllEnrollmentsAsync();

            // Assert: Admin can see the student-created enrollment
            Assert.That(adminEnrollments.Count(), Is.EqualTo(1), "Admin should see 1 enrollment");
            var adminEnrollment = adminEnrollments.First();
            Assert.That(adminEnrollment.UserId, Is.EqualTo(userId), "Admin should see the correct user");
            Assert.That(adminEnrollment.CourseId, Is.EqualTo(courseId), "Admin should see the correct course");
        }

        [Test]
        public async Task DuplicateEnrollment_ShouldBePrevented()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Act: Admin enrolls user first
            var adminEnrollmentResult = await _adminService.EnrollUserInCourseAsync(userId, courseId);
            Assert.That(adminEnrollmentResult, Is.True, "Admin enrollment should succeed");

            // Act: Student tries to enroll in the same course
            var studentEnrollmentResult = await _enrollmentService.EnrollStudentInCourseAsync(userId, courseId);

            // Assert: Student enrollment should fail (already enrolled)
            Assert.That(studentEnrollmentResult, Is.False, "Student should not be able to enroll in already enrolled course");

            // Act: Admin tries to enroll user again
            var adminEnrollmentResult2 = await _adminService.EnrollUserInCourseAsync(userId, courseId);

            // Assert: Admin enrollment should also fail
            Assert.That(adminEnrollmentResult2, Is.False, "Admin should not be able to enroll user in already enrolled course");
        }

        [Test]
        public async Task AvailableCourses_ShouldExcludeEnrolledCourses()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Create additional course
            var additionalCourse = new Course
            {
                CourseId = 2,
                CourseTitle = "Additional Course",
                CourseCode = "ADD101",
                Description = "An additional course",
                Credits = 4
            };
            _context.Courses.Add(additionalCourse);
            await _context.SaveChangesAsync();

            // Act: Admin enrolls user in first course
            var adminEnrollmentResult = await _adminService.EnrollUserInCourseAsync(userId, courseId);
            Assert.That(adminEnrollmentResult, Is.True, "Admin enrollment should succeed");

            // Act: Student checks available courses
            var availableCourses = await _enrollmentService.GetAvailableCoursesByStudentIdAsync(userId);

            // Assert: Enrolled course should not be in available courses
            Assert.That(availableCourses.Any(c => c.CourseId == courseId), Is.False, "Enrolled course should not be available");
            Assert.That(availableCourses.Any(c => c.CourseId == additionalCourse.CourseId), Is.True, "Non-enrolled course should be available");
        }

        [Test]
        public async Task Deregistration_ShouldWorkFromBothServices()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Admin enrolls user
            var adminEnrollmentResult = await _adminService.EnrollUserInCourseAsync(userId, courseId);
            Assert.That(adminEnrollmentResult, Is.True, "Admin enrollment should succeed");

            // Verify enrollment exists
            var studentEnrollments = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(studentEnrollments.Count(), Is.EqualTo(1), "Student should see 1 enrollment");

            // Act: Admin deregisters user
            var adminDeregistrationResult = await _adminService.DeregisterUserFromCourseAsync(userId, courseId);

            // Assert: Admin deregistration was successful
            Assert.That(adminDeregistrationResult, Is.True, "Admin should be able to deregister user");

            // Verify enrollment no longer exists for student
            var studentEnrollmentsAfter = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(studentEnrollmentsAfter.Count(), Is.EqualTo(0), "Student should see 0 enrollments after deregistration");
        }

        [Test]
        public async Task StudentDeregistration_ShouldWorkAndBeVisibleToAdmin()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Student enrolls themselves
            var studentEnrollmentResult = await _enrollmentService.EnrollStudentInCourseAsync(userId, courseId);
            Assert.That(studentEnrollmentResult, Is.True, "Student enrollment should succeed");

            // Verify enrollment exists for admin
            var adminEnrollments = await _adminService.GetAllEnrollmentsAsync();
            Assert.That(adminEnrollments.Count(), Is.EqualTo(1), "Admin should see 1 enrollment");

            // Act: Student deregisters themselves
            var studentDeregistrationResult = await _enrollmentService.DeregisterStudentFromCourseAsync(userId, courseId);

            // Assert: Student deregistration was successful
            Assert.That(studentDeregistrationResult, Is.True, "Student should be able to deregister themselves");

            // Verify enrollment no longer exists for admin
            var adminEnrollmentsAfter = await _adminService.GetAllEnrollmentsAsync();
            Assert.That(adminEnrollmentsAfter.Count(), Is.EqualTo(0), "Admin should see 0 enrollments after deregistration");
        }

        [Test]
        public async Task CourseEnrollmentSummary_ShouldShowCorrectEnrollments()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId = _testCourse.CourseId;

            // Admin enrolls user
            var adminEnrollmentResult = await _adminService.EnrollUserInCourseAsync(userId, courseId);
            Assert.That(adminEnrollmentResult, Is.True, "Admin enrollment should succeed");

            // Act: Get course enrollment summary
            var summary = await _adminService.GetCourseEnrollmentSummaryAsync(courseId);

            // Assert: Summary should show correct information
            Assert.That(summary.CourseId, Is.EqualTo(courseId), "Summary should have correct course ID");
            Assert.That(summary.TotalEnrollments, Is.EqualTo(1), "Summary should show 1 enrollment");
            Assert.That(summary.EnrolledUsers.Count, Is.EqualTo(1), "Summary should show 1 enrolled user");
            
            var enrolledUser = summary.EnrolledUsers.First();
            Assert.That(enrolledUser.UserId, Is.EqualTo(userId), "Summary should show correct user ID");
            Assert.That(enrolledUser.Email, Is.EqualTo(_testUser.Email), "Summary should show correct user email");
        }

        [Test]
        public async Task MultipleEnrollments_ShouldBeSynchronized()
        {
            // Arrange
            var userId = _testUser.Id;
            var courseId1 = _testCourse.CourseId;

            // Create second course
            var course2 = new Course
            {
                CourseId = 2,
                CourseTitle = "Second Course",
                CourseCode = "SEC101",
                Description = "A second course",
                Credits = 3
            };
            _context.Courses.Add(course2);
            await _context.SaveChangesAsync();

            // Act: Admin enrolls user in first course
            var adminEnrollment1 = await _adminService.EnrollUserInCourseAsync(userId, courseId1);
            Assert.That(adminEnrollment1, Is.True, "First admin enrollment should succeed");

            // Act: Student enrolls themselves in second course
            var studentEnrollment2 = await _enrollmentService.EnrollStudentInCourseAsync(userId, course2.CourseId);
            Assert.That(studentEnrollment2, Is.True, "Second student enrollment should succeed");

            // Assert: Both enrollments should be visible to student
            var studentEnrollments = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(userId);
            Assert.That(studentEnrollments.Count(), Is.EqualTo(2), "Student should see 2 enrollments");

            // Assert: Both enrollments should be visible to admin
            var adminEnrollments = await _adminService.GetAllEnrollmentsAsync();
            Assert.That(adminEnrollments.Count(), Is.EqualTo(2), "Admin should see 2 enrollments");

            // Verify specific courses
            var studentCourseIds = studentEnrollments.Select(c => c.CourseId).ToList();
            Assert.That(studentCourseIds, Does.Contain(courseId1), "Student should see first course");
            Assert.That(studentCourseIds, Does.Contain(course2.CourseId), "Student should see second course");

            var adminCourseIds = adminEnrollments.Select(e => e.CourseId).ToList();
            Assert.That(adminCourseIds, Does.Contain(courseId1), "Admin should see first course");
            Assert.That(adminCourseIds, Does.Contain(course2.CourseId), "Admin should see second course");
        }
    }
}
