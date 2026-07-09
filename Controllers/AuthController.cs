using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.DTOs;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Services;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, ILogger<TraineeController> logger) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<TraineeController> _logger = logger;

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> RegisterUser([FromBody] UserRegisterRequest registerRequest)
        {
            try
            {
                UserResponse userResponse = await _authService.RegisterUserAsync(registerRequest);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterUser for Username {Username}", registerRequest.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while registering user with Username {registerRequest.Username}.");
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> LoginUser([FromBody] UserLoginRequest loginRequest)
        {
            try
            {
                UserLoginResponse userResponse = await _authService.LoginUserAsync(loginRequest);
                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginUser for Username {Username}", loginRequest.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, "Invalid Username or Password.");
            }
        }
    }
}