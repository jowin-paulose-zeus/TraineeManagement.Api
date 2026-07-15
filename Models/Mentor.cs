using System.Diagnostics.CodeAnalysis;
using TraineeManagement.Api.Enums;
using System.Text.Json.Serialization;

namespace TraineeManagement.Api.Models
{
    public class Mentor
    {
        [SetsRequiredMembers]
        public Mentor(string firstName, string lastName, string email, string expertise, MentorStatus status)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Expertise = expertise;
            Status = status;
        }
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Expertise { get; set; }
        public required MentorStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public ICollection<TaskAssignment> TaskAssignments { get; set; } = [];

    }
}