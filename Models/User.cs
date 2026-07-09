using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;
using System.Diagnostics.CodeAnalysis;

namespace TraineeManagement.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required UserRoles Role { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [SetsRequiredMembers]
        public User(string username, string email, string passwordHash, UserRoles role)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}