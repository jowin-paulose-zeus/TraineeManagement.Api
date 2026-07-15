using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class TaskAssignmentResponse
    {
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public required string TraineeName { get; set; } 
        public int MentorId { get; set; }
        public required string MentorName { get; set;}
        public int LearningTaskId { get; set; }
        public required string LearningTaskTitle { get; set;}
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public required string Status { get; set; }
        public string? Remarks { get; set; }
    }
}
