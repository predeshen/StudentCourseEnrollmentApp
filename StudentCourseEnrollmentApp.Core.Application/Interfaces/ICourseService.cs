using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Core.Application.Services
{
    public interface ICourseService
    {
        Task<CourseDTO> GetCourseByIdAsync(int courseId);
        Task<IEnumerable<CourseDTO>> GetAllCoursesAsync();
        Task<CourseDTO> AddCourseAsync(CourseDTO courseDto);
        Task<bool> UpdateCourseAsync(CourseDTO courseDto);
        Task<bool> DeleteCourseAsync(int courseId);
    }
}