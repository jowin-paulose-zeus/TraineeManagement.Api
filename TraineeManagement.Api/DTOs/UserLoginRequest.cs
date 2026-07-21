using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Api.DTOs
{
    public class UserLoginRequest
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}