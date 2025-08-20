using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Interfaces;
using StudentCourseEnrollmentApp.Core.Domain;
using StudentCourseEnrollmentApp.Infrastructure.Data;

namespace StudentCourseEnrollmentApp.Infrastructure.Data.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Course Management
        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO createCourseDto)
        {
            var course = new Course
            {
                CourseTitle = createCourseDto.CourseTitle,
                CourseCode = createCourseDto.CourseCode,
                Description = createCourseDto.Description,
                Credits = createCourseDto.Credits
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return new CourseDTO
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseCode = course.CourseCode,
                Description = course.Description,
                Credits = course.Credits
            };
        }

        public async Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO updateCourseDto)
        {
            var course = await _context.Courses.FindAsync(updateCourseDto.CourseId);
            if (course == null)
                throw new InvalidOperationException("Course not found");

            course.CourseTitle = updateCourseDto.CourseTitle;
            course.CourseCode = updateCourseDto.CourseCode;
            course.Description = updateCourseDto.Description;
            course.Credits = updateCourseDto.Credits;

            await _context.SaveChangesAsync();

            return new CourseDTO
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseCode = course.CourseCode,
                Description = course.Description,
                Credits = course.Credits
            };
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return false;

            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.CourseId == courseId);
            if (hasEnrollments)
                return false; // Cannot delete course with enrollments

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseDTO?> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return null;

            return new CourseDTO
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseCode = course.CourseCode,
                Description = course.Description,
                Credits = course.Credits
            };
        }

        // User Management
        public async Task<ApplicationUserDTO> CreateUserAsync(CreateUserDTO createUserDto)
        {
            var user = new ApplicationUser
            {
                UserName = createUserDto.Email,
                Email = createUserDto.Email,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                IsSuperAdmin = createUserDto.IsSuperAdmin,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return new ApplicationUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsSuperAdmin = user.IsSuperAdmin,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<ApplicationUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.UserId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.Email;
            user.IsSuperAdmin = updateUserDto.IsSuperAdmin;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return new ApplicationUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsSuperAdmin = user.IsSuperAdmin,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var hasEnrollments = await _context.Enrollments.AnyAsync(e => e.UserId == userId);
            if (hasEnrollments)
                return false; // Cannot delete user with enrollments

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new ApplicationUserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsSuperAdmin = u.IsSuperAdmin,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<ApplicationUserDTO?> GetUserByIdAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            return new ApplicationUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsSuperAdmin = user.IsSuperAdmin,
                CreatedAt = user.CreatedAt
            };
        }

        // Enrollment Management
        public async Task<bool> EnrollUserInCourseAsync(string userId, int courseId)
        {
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (isEnrolled)
                return false;

            var enrollment = new Enrollment
            {
                UserId = userId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeregisterUserFromCourseAsync(string userId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return false;

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseEnrollmentSummaryDTO> GetCourseEnrollmentSummaryAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                throw new InvalidOperationException("Course not found");

            var enrollments = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.User)
                .ToListAsync();

            var enrolledUsers = enrollments.Select(e => new UserEnrollmentDTO
            {
                UserId = e.UserId,
                Email = e.User.Email,
                FirstName = e.User.FirstName,
                LastName = e.User.LastName,
                EnrollmentDate = e.EnrollmentDate,
                IsSuperAdmin = e.User.IsSuperAdmin
            }).ToList();

            return new CourseEnrollmentSummaryDTO
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseCode = course.CourseCode,
                TotalEnrollments = enrollments.Count,
                EnrolledUsers = enrolledUsers
            };
        }

        public async Task<IEnumerable<AdminEnrollmentDTO>> GetAllEnrollmentsAsync()
        {
            return await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .Select(e => new AdminEnrollmentDTO
                {
                    UserId = e.UserId,
                    CourseId = e.CourseId,
                    UserEmail = e.User.Email,
                    UserName = $"{e.User.FirstName} {e.User.LastName}",
                    CourseTitle = e.Course.CourseTitle,
                    EnrollmentDate = e.EnrollmentDate
                })
                .ToListAsync();
        }
    }
}
