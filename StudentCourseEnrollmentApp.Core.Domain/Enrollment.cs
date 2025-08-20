using System;

namespace StudentCourseEnrollmentApp.Core.Domain
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public ApplicationUser? User { get; set; }
        public Course? Course { get; set; }
    }
}