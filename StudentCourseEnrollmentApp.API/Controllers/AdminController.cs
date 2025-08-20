using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Interfaces;
using System.Security.Claims;

namespace StudentCourseEnrollmentApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        private bool IsSuperAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return false;
            var isSuperAdminClaim = User.FindFirstValue("IsSuperAdmin");
            return bool.TryParse(isSuperAdminClaim, out var isAdmin) && isAdmin;
        }

        // Course Management
        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDTO createCourseDto)
        {
            if (!IsSuperAdmin())
                return Forbid();

            try
            {
                var course = await _adminService.CreateCourseAsync(createCourseDto);
                return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, course);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("courses")]
        public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseDTO updateCourseDto)
        {
            if (!IsSuperAdmin())
                return Forbid();

            try
            {
                var course = await _adminService.UpdateCourseAsync(updateCourseDto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("courses/{courseId}")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {
            if (!IsSuperAdmin())
                return Forbid();

            var result = await _adminService.DeleteCourseAsync(courseId);
            if (!result)
                return BadRequest("Cannot delete course with enrollments or course not found");

            return Ok("Course deleted successfully");
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetCourseById(int id)
        {
            if (!IsSuperAdmin())
                return Forbid();

            var course = await _adminService.GetCourseByIdAsync(id);
            if (course == null)
                return NotFound();

            return Ok(course);
        }

        // User Management
        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            if (!IsSuperAdmin())
                return Forbid();

            try
            {
                var user = await _adminService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("users")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO updateUserDto)
        {
            if (!IsSuperAdmin())
                return Forbid();

            try
            {
                var user = await _adminService.UpdateUserAsync(updateUserDto);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (!IsSuperAdmin())
                return Forbid();

            var result = await _adminService.DeleteUserAsync(userId);
            if (!result)
                return BadRequest("Cannot delete user with enrollments or user not found");

            return Ok("User deleted successfully");
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsSuperAdmin())
                return Forbid();

            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (!IsSuperAdmin())
                return Forbid();

            var user = await _adminService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // Enrollment Management
        [HttpPost("enrollments")]
        public async Task<IActionResult> EnrollUserInCourse([FromBody] AdminEnrollmentRequestDTO request)
        {
            if (!IsSuperAdmin())
                return Forbid();

            var result = await _adminService.EnrollUserInCourseAsync(request.UserId, request.CourseId);
            if (!result)
                return BadRequest("User is already enrolled in this course or invalid request");

            return Ok("User enrolled successfully");
        }

        [HttpDelete("enrollments")]
        public async Task<IActionResult> DeregisterUserFromCourse([FromBody] AdminEnrollmentRequestDTO request)
        {
            if (!IsSuperAdmin())
                return Forbid();

            var result = await _adminService.DeregisterUserFromCourseAsync(request.UserId, request.CourseId);
            if (!result)
                return BadRequest("Enrollment not found or invalid request");

            return Ok("User deregistered successfully");
        }

        [HttpGet("enrollments/course/{courseId}")]
        public async Task<IActionResult> GetCourseEnrollmentSummary(int courseId)
        {
            if (!IsSuperAdmin())
                return Forbid();

            try
            {
                var summary = await _adminService.GetCourseEnrollmentSummaryAsync(courseId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("enrollments")]
        public async Task<IActionResult> GetAllEnrollments()
        {
            if (!IsSuperAdmin())
                return Forbid();

            var enrollments = await _adminService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }
    }

    public class AdminEnrollmentRequestDTO
    {
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}
