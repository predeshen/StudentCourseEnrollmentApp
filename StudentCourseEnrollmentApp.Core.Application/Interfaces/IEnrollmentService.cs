using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Core.Application.Services
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollStudentInCourseAsync(string userId, int courseId);
        Task<bool> DeregisterStudentFromCourseAsync(string userId, int courseId);
        Task<IEnumerable<CourseDTO>> GetEnrolledCoursesByStudentIdAsync(string userId);
        Task<IEnumerable<CourseDTO>> GetAvailableCoursesByStudentIdAsync(string userId);
    }
}