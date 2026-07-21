namespace TraineeManagement.Api.DTOs
{
    public class UserLoginResponse
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string Role { get; set; }
        public required string Token { get; set; }
        public required int ExpiresInMinutes { get; set; }
    }
}