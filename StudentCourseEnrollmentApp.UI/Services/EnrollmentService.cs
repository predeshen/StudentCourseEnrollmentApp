using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StudentCourseEnrollmentApp.UI.Services.Interfaces;

namespace StudentCourseEnrollmentApp.UI.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly HttpClient _httpClient;

        public EnrollmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> EnrollInCourseAsync(int courseId)
        {
            var response = await _httpClient.PostAsync($"api/enrollments/enroll/{courseId}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeregisterFromCourseAsync(int courseId)
        {
            var response = await _httpClient.DeleteAsync($"api/enrollments/deregister/{courseId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<CourseDTO>> GetMyCoursesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<CourseDTO>>("api/enrollments/my-courses");
        }
    }
}