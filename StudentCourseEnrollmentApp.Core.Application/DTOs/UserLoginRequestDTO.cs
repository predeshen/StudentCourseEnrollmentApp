using System.ComponentModel.DataAnnotations;

namespace StudentCourseEnrollmentApp.Core.Application.DTOs
{
    public class UserLoginRequestDTO
    {
        [EmailAddress]
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}