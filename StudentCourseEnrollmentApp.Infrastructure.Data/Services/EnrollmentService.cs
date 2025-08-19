using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Services;
using StudentCourseEnrollmentApp.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.Infrastructure.Data.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> EnrollStudentInCourseAsync(int studentId, int courseId)
        {
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (isEnrolled)
            {
                return false;
            }

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeregisterStudentFromCourseAsync(int studentId, int courseId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                return false;
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<CourseDTO>> GetEnrolledCoursesByStudentIdAsync(int studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course) 
                .Select(e => new CourseDTO
                {
                    CourseId = e.Course.CourseId,
                    CourseTitle = e.Course.CourseTitle,
                    CourseCode = e.Course.CourseCode,
                    Description = e.Course.Description,
                    Credits = e.Course.Credits
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseDTO>> GetAvailableCoursesByStudentIdAsync(int studentId)
        {
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            return await _context.Courses
                .Where(c => !enrolledCourseIds.Contains(c.CourseId))
                .Select(c => new CourseDTO
                {
                    CourseId = c.CourseId,
                    CourseTitle = c.CourseTitle,
                    CourseCode = c.CourseCode,
                    Description = c.Description,
                    Credits = c.Credits
                })
                .ToListAsync();
        }
    }
}