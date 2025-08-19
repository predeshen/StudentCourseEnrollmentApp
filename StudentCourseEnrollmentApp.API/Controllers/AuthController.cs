using Microsoft.AspNetCore.Mvc;
using StudentCourseEnrollmentApp.Core.Application;
using StudentCourseEnrollmentApp.Core.Application.DTOs;

namespace StudentCourseEnrollmentApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.RegisterUserAsync(request);

            if (!result.Result)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticationService.LoginUserAsync(request);

            if (!result.Result)
            {
                return Unauthorized(new { errors = result.Errors });
            }

            return Ok(result);
        }
    }
}