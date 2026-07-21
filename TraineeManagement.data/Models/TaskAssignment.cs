using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Data.Models
{
    public class TaskAssignment
    {
        public TaskAssignment() {}
        [SetsRequiredMembers]
        public TaskAssignment(Trainee trainee, Mentor mentor, LearningTask learningTask, DateTime assignedDate, DateTime dueDate, TaskAssignmentStatus status, string? remark = null)
        {
            TraineeId = trainee.Id;
            Trainee = trainee;
            MentorId = mentor.Id;
            Mentor = mentor;
            LearningTaskId = learningTask.Id;
            LearningTask = learningTask;
            AssignedDate = assignedDate;
            DueDate = dueDate;
            Status = status;
            Remarks = remark;
        }
        public int Id { get; set; }
        public int TraineeId { get; set; }
        public required Trainee Trainee { get; set; }
        public int MentorId { get; set; }
        public required Mentor Mentor { get; set; }
        public int LearningTaskId { get; set; }
        public required LearningTask LearningTask { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public TaskAssignmentStatus Status { get; set; }
        public string? Remarks { get; set; }
        [JsonIgnore]
        public Submission? Submission { get; set; }
    }
}