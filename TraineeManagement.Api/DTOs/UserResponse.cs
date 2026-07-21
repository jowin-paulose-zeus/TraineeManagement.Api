using TraineeManagement.Data.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class UserResponse
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}