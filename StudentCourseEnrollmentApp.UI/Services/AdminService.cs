using StudentCourseEnrollmentApp.Core.Application.DTOs;
using StudentCourseEnrollmentApp.Core.Application.Interfaces;
using System.Net.Http.Json;

namespace StudentCourseEnrollmentApp.UI.Services
{
    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;

        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Course Management
        public async Task<CourseDTO> CreateCourseAsync(CreateCourseDTO createCourseDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/admin/courses", createCourseDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseDTO>() ?? new CourseDTO();
        }

        public async Task<CourseDTO> UpdateCourseAsync(UpdateCourseDTO updateCourseDto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/admin/courses", updateCourseDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseDTO>() ?? new CourseDTO();
        }

        public async Task<bool> DeleteCourseAsync(int courseId)
        {
            var response = await _httpClient.DeleteAsync($"api/admin/courses/{courseId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<CourseDTO?> GetCourseByIdAsync(int courseId)
        {
            var response = await _httpClient.GetAsync($"api/admin/courses/{courseId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CourseDTO>();
            }
            return null;
        }

        // User Management
        public async Task<ApplicationUserDTO> CreateUserAsync(CreateUserDTO createUserDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/admin/users", createUserDto);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API Error: {response.StatusCode} - {errorContent}");
                }
                
                var result = await response.Content.ReadFromJsonAsync<ApplicationUserDTO>();
                return result ?? new ApplicationUserDTO();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApplicationUserDTO> UpdateUserAsync(UpdateUserDTO updateUserDto)
        {
            var response = await _httpClient.PutAsJsonAsync("api/admin/users", updateUserDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApplicationUserDTO>() ?? new ApplicationUserDTO();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var response = await _httpClient.DeleteAsync($"api/admin/users/{userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<IEnumerable<ApplicationUserDTO>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("api/admin/users");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ApplicationUserDTO>>() ?? new List<ApplicationUserDTO>();
        }

        public async Task<ApplicationUserDTO?> GetUserByIdAsync(string userId)
        {
            var response = await _httpClient.GetAsync($"api/admin/users/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApplicationUserDTO>();
            }
            return null;
        }

        // Enrollment Management
        public async Task<bool> EnrollUserInCourseAsync(string userId, int courseId)
        {
            var request = new { UserId = userId, CourseId = courseId };
            var response = await _httpClient.PostAsJsonAsync("api/admin/enrollments", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeregisterUserFromCourseAsync(string userId, int courseId)
        {
            var request = new { UserId = userId, CourseId = courseId };
            var response = await _httpClient.DeleteAsync($"api/admin/enrollments?userId={userId}&courseId={courseId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<CourseEnrollmentSummaryDTO> GetCourseEnrollmentSummaryAsync(int courseId)
        {
            var response = await _httpClient.GetAsync($"api/admin/enrollments/course/{courseId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CourseEnrollmentSummaryDTO>() ?? new CourseEnrollmentSummaryDTO();
        }

        public async Task<IEnumerable<AdminEnrollmentDTO>> GetAllEnrollmentsAsync()
        {
            var response = await _httpClient.GetAsync("api/admin/enrollments");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AdminEnrollmentDTO>>() ?? new List<AdminEnrollmentDTO>();
        }
    }
}
