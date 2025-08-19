namespace StudentCourseEnrollmentApp.Core.Application.DTOs
{
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseTitle { get; set; }
        public string? Description { get; set; }
        public int Credits { get; set; }
    }
}