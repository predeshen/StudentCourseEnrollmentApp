using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.UI.Services.Interfaces;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StudentCourseEnrollmentApp.UI.Services
{
    public class CourseService : ICourseService
    {
        private readonly HttpClient _httpClient;

        public CourseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CourseDTO>>("api/courses");
        }
    }
}