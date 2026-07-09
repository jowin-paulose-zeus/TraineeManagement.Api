using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
