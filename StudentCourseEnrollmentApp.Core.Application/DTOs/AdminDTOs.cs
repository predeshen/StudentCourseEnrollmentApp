using System.ComponentModel.DataAnnotations;

namespace StudentCourseEnrollmentApp.Core.Application.DTOs
{
    public class CreateCourseDTO
    {
        [Required]
        [StringLength(100)]
        public string CourseTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Credits { get; set; }
    }

    public class UpdateCourseDTO
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Credits { get; set; }
    }

    public class CreateUserDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public bool IsSuperAdmin { get; set; } = false;
    }

    public class UpdateUserDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool IsSuperAdmin { get; set; } = false;
    }

    public class UserEnrollmentDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public bool IsSuperAdmin { get; set; }
    }

    public class CourseEnrollmentSummaryDTO
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int TotalEnrollments { get; set; }
        public List<UserEnrollmentDTO> EnrolledUsers { get; set; } = new List<UserEnrollmentDTO>();
    }

    public class AdminEnrollmentDTO
    {
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
    }

    public class ApplicationUserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
