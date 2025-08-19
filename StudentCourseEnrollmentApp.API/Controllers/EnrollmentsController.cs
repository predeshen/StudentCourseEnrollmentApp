using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpPost("enroll/{courseId}")]
        public async Task<IActionResult> EnrollInCourse(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            int studentId = int.Parse(userId);

            var result = await _enrollmentService.EnrollStudentInCourseAsync(studentId, courseId);
            if (!result)
            {
                return BadRequest("Enrollment failed. You may already be enrolled in this course.");
            }

            return Ok("Enrollment successful.");
        }

        [HttpDelete("deregister/{courseId}")]
        public async Task<IActionResult> DeregisterFromCourse(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            int studentId = int.Parse(userId);

            var result = await _enrollmentService.DeregisterStudentFromCourseAsync(studentId, courseId);
            if (!result)
            {
                return NotFound("Enrollment not found.");
            }

            return Ok("Deregistration successful.");
        }

        [HttpGet("my-courses")]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetMyEnrolledCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            int studentId = int.Parse(userId);

            var courses = await _enrollmentService.GetEnrolledCoursesByStudentIdAsync(studentId);
            return Ok(courses);
        }

        [HttpGet("available-courses")]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAvailableCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            int studentId = int.Parse(userId);

            var courses = await _enrollmentService.GetAvailableCoursesByStudentIdAsync(studentId);
            return Ok(courses);
        }
    }
}