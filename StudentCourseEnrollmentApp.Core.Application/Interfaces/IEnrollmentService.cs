using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Core.Application.Services
{
    public interface IEnrollmentService
    {
        Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId);
        Task<bool> DeregisterStudentFromCourseAsync(int studentId, int courseId);
        Task<IEnumerable<CourseDTO>> GetEnrolledCoursesByStudentIdAsync(int studentId);
        Task<IEnumerable<CourseDTO>> GetAvailableCoursesByStudentIdAsync(int studentId);
    }
}