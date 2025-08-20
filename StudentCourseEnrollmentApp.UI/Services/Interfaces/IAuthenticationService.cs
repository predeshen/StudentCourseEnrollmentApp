using StudentCourseEnrollmentApp.Core.Application.DTOs;

namespace StudentCourseEnrollmentApp.UI.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResultDTO> RegisterUserAsync(UserRegistrationRequestDTO request);
        Task<AuthResultDTO> LoginUserAsync(UserLoginRequestDTO request);
    }
}