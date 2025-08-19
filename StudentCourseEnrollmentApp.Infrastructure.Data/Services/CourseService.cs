using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Services;
using StudentCourseEnrollmentApp.Core.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Infrastructure.Data.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext _context;

        public CourseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CourseDTO> GetCourseByIdAsync(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            return course == null ? null : new CourseDTO
            {
                CourseId = course.CourseId,
                CourseTitle = course.CourseTitle,
                CourseCode = course.CourseCode,
                Description = course.Description,
                Credits = course.Credits
            };
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    CourseCode = c.CourseCode,
                    Description = c.Description,
                    Credits = c.Credits
                }).ToListAsync();
        }

        Task<CourseDTO> ICourseService.AddCourseAsync(CourseDTO courseDto)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICourseService.UpdateCourseAsync(CourseDTO courseDto)
        {
            throw new NotImplementedException();
        }

        Task<bool> ICourseService.DeleteCourseAsync(int courseId)
        {
            throw new NotImplementedException();
        }
    }
}