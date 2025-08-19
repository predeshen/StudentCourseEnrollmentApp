using System.ComponentModel.DataAnnotations;

namespace StudentCourseEnrollmentApp.Core.Application.DTOs
{
    public class UserRegistrationRequestDTO
    {
        [EmailAddress]
        public string? Email { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }
    }
}