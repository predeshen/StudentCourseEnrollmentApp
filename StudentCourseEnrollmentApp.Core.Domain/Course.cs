using System.Collections.Generic;

namespace StudentCourseEnrollmentApp.Core.Domain
{
    public class Course
    {
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public string? CourseCode { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}