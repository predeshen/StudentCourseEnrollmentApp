using StudentCourseEnrollmentApp.Core.Application.DTOs;

namespace StudentCourseEnrollmentApp.UI.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResultDTO> RegisterAsync(UserRegistrationRequestDTO user);
        Task<AuthResultDTO> LoginAsync(UserLoginRequestDTO user);
    }
}