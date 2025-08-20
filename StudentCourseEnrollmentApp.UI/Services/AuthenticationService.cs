using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Net.Http.Json;
using StudentCourseEnrollmentApp.UI.Services.Interfaces;

namespace StudentCourseEnrollmentApp.UI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResultDTO> RegisterUserAsync(UserRegistrationRequestDTO request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            var authResult = await result.Content.ReadFromJsonAsync<AuthResultDTO>();
            return authResult;
        }

        public async Task<AuthResultDTO> LoginUserAsync(UserLoginRequestDTO request)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            var authResult = await result.Content.ReadFromJsonAsync<AuthResultDTO>();
            return authResult;
        }
    }
}