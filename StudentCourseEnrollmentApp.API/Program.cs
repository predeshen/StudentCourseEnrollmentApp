using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentCourseEnrollmentApp.Core.Domain;
using StudentCourseEnrollmentApp.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://localhost:7002", "https://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Call our custom extension methods to register services and JWT authentication
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    context.Database.EnsureCreated();

    if (!context.Courses.Any())
    {
        context.Courses.AddRange(
            new Course { CourseId = 1, CourseTitle = "English For Beginners", CourseCode = "EFB101", Description = "An introduction to the english language.", Credits = 3 },
            new Course { CourseId = 2, CourseTitle = "Mathematic Fundamentals", CourseCode = "MF101", Description = "Introduction to mathematics", Credits = 4 },
            new Course { CourseId = 3, CourseTitle = "Afrikaans Fundamentals", CourseCode = "AF101", Description = "An introduction to the afrikaans language.", Credits = 5 }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowBlazorApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();