using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace StudentCourseEnrollmentApp.Infrastructure.Data
{
    // Inherit from IdentityDbContext to use ASP.NET Core Identity
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for our custom domain entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base method to configure Identity tables
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and keys
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            // Seed some initial data for testing
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, CourseTitle = "English For Beginners", CourseCode = "EFB101", Description = "An introduction to the english language.", Credits = 3 },
                new Course { CourseId = 2, CourseTitle = "Mathematic Fundamentals", CourseCode = "MF101", Description = "Introduction to mathematics", Credits = 4 },
                new Course { CourseId = 3, CourseTitle = "Afrikaans Fundamentals", CourseCode = "AF101", Description = "An introduction to the afrikaans language.", Credits = 5 }
            );
        }
    }
}