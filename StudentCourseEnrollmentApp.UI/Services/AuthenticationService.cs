using StudentCourseEnrollmentApp.Core.Application.DTOs;
using System.Net.Http.Json;
using StudentCourseEnrollmentApp.UI.Services.Interfaces;

namespace StudentCourseEnrollmentApp.Client.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResultDTO> RegisterAsync(UserRegistrationRequestDTO user)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/register", user);
            var authResult = await result.Content.ReadFromJsonAsync<AuthResultDTO>();
            return authResult;
        }

        public async Task<AuthResultDTO> LoginAsync(UserLoginRequestDTO user)
        {
            var result = await _httpClient.PostAsJsonAsync("api/auth/login", user);
            var authResult = await result.Content.ReadFromJsonAsync<AuthResultDTO>();
            return authResult;
        }
    }
}