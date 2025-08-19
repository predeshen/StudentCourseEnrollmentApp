using System;

namespace StudentCourseEnrollmentApp.Core.Domain
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}