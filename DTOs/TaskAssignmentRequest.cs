using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class TaskAssignmentRequest
    {
        public int TraineeId { get; set; } 
        public int MentorId { get; set; }
        public int LearningTaskId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public TaskAssignmentStatus Status { get; set; }
        public string? Remarks { get; set; }
    }
}
