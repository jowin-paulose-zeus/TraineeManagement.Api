using TraineeManagement.Data.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
