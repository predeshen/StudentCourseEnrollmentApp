using System.Threading.Tasks;
using StudentCourseEnrollmentApp.Core.Application.DTOs;

namespace StudentCourseEnrollmentApp.Core.Application
{
    public interface IAuthenticationService
    {
        Task<AuthResultDTO> RegisterUserAsync(UserRegistrationRequestDTO request);

        Task<AuthResultDTO> LoginUserAsync(UserLoginRequestDTO request);
    }
}