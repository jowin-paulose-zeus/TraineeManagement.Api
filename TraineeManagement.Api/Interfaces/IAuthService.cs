using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterUserAsync(UserRegisterRequest registerRequest);
        Task<UserLoginResponse> LoginUserAsync(UserLoginRequest loginRequest);
    }
}
