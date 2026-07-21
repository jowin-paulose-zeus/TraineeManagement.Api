using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Data.Data;
using TraineeManagement.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService,ILogger<AuthService> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthService> _logger = logger;

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> RegisterUser([FromBody] UserRegisterRequest registerRequest)
        {
            try
            {
                UserResponse userResponse = await _authService.RegisterUserAsync(registerRequest);
                _logger.LogInformation("User '{Username}' registered in successfully", userResponse.Username);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error in Registering User for Username {Username}", registerRequest.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while registering user with Username {registerRequest.Username}.");
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> LoginUser([FromBody] UserLoginRequest loginRequest)
        {
            try
            {
                UserLoginResponse userResponse = await _authService.LoginUserAsync(loginRequest);
                _logger.LogInformation("User '{Username}' logged in successfully", userResponse.Username);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Loging User for Username {Username}", loginRequest.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Invalid Username or Password.");
            }
        }
    }
}