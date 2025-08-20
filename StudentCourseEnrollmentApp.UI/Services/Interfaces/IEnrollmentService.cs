using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.UI.Services.Interfaces

{
    public interface IEnrollmentService
    {
        Task<bool> EnrollInCourseAsync(int courseId);
        Task<bool> DeregisterFromCourseAsync(int courseId);
        Task<IEnumerable<CourseDTO>> GetMyCoursesAsync();
    }
}