using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.Data;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Data.Enums;
using TraineeManagement.Data.Models;
using BCrypt.Net;
using TraineeManagement.Api.Interfaces;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Configuration;


namespace TraineeManagement.Api.Services
{
    public class AuthService(TraineeDbContext context, IJwtService jwtService, IOptions<JwtSettings> jwtSettings) : IAuthService
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly TraineeDbContext _context = context;
        private readonly IOptions<JwtSettings> _jwtSettings = jwtSettings;
        private static UserResponse MapToResponse(User user)
        {
            return new()
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
        public async Task<UserResponse> RegisterUserAsync(UserRegisterRequest registerRequest)
        {
            bool usernameExists = await _context.Users
                .AnyAsync(user => user.Username == registerRequest.Username);

            if (usernameExists)
            {
                throw new Exception("Username already exists.");
            }

            bool emailExists = await _context.Users
                .AnyAsync(user => user.Email == registerRequest.Email);

            if (emailExists)
            {
                throw new Exception("Email already exists.");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
            User user = new(registerRequest.Username, registerRequest.Email, hashedPassword, registerRequest.Role)
            {
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToResponse(user);
        }
        public async Task<UserLoginResponse> LoginUserAsync(UserLoginRequest loginRequest)
        {
            User? user = await _context.Users
                .FirstOrDefaultAsync(user => user.Username == loginRequest.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password.");
            }
            string token = _jwtService.GenerateToken(user);

            return new UserLoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role.ToString(),
                Token = token,
                ExpiresInMinutes = _jwtSettings.Value.ExpiresInMinutes
            };
        }
    }
}
