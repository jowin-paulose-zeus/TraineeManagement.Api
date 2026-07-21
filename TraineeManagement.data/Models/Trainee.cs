using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Data.Models
{
    public class Trainee
    {
        [SetsRequiredMembers]
        public Trainee(string firstName, string lastName, string email, string techStack, TraineeStatus status)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            TechStack = techStack;
            Status = status;
        }
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string TechStack { get; set; }
        public required TraineeStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public ICollection<TaskAssignment> TaskAssignments { get; set; } = [];

    }
}