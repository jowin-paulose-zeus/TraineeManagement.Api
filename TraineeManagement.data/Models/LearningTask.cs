using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Data.Models
{
    public class LearningTask
    {
        [SetsRequiredMembers]
        public LearningTask(string title, string description, string expectedTechStack, DateTime dueDate, LearningTaskStatus status)
        {
            Title = title;
            Description = description;
            ExpectedTechStack = expectedTechStack;
            DueDate = dueDate;
            Status = status;
        }
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public DateTime DueDate { get; set; }
        public required LearningTaskStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public ICollection<TaskAssignment> TaskAssignments { get; set; } = [];

    }
}