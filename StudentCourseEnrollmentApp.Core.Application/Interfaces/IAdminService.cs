using StudentCourseEnrollmentApp.Core.Application.DTOs;

namespace StudentCourseEnrollmentApp.Core.Application.Interfaces
{
    public interface IAdminService
    {
        // Course Management
        Task<CourseDTO> CreateCourseAsync(CreateCourseDTO createCourseDto);
        Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO updateCourseDto);
        Task<bool> DeleteCourseAsync(int courseId);
        Task<CourseDTO?> GetCourseByIdAsync(int courseId);
        
        // User Management
        Task<ApplicationUserDTO> CreateUserAsync(CreateUserDTO createUserDto);
        Task<ApplicationUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync();
        Task<ApplicationUserDTO?> GetUserByIdAsync(string userId);
        
        // Enrollment Management
        Task<bool> EnrollUserInCourseAsync(string userId, int courseId);
        Task<bool> DeregisterUserFromCourseAsync(string userId, int courseId);
        Task<CourseEnrollmentSummaryDTO> GetCourseEnrollmentSummaryAsync(int courseId);
        Task<IEnumerable<AdminEnrollmentDTO>> GetAllEnrollmentsAsync();
    }
}
