using System.Diagnostics.CodeAnalysis;
using Org.BouncyCastle.Bcpg;
using TraineeManagement.Api.Enums;

namespace TraineeManagement.Api.Models
{

    public class Submission
    {
        public Submission() { }
        [SetsRequiredMembers]
        public Submission(TaskAssignment taskAssignment, string submissionUrl,SubmissionStatus status, string? notes = null)
        {
            TaskAssignmentId = taskAssignment.Id;
            TaskAssignment = taskAssignment;
            SubmissionUrl = submissionUrl;
            Notes = notes;
            SubmissionDate = DateTime.UtcNow;
            Status = status;
        }
        public int Id { get; set; }
        public int TaskAssignmentId { get; set; }
        public required TaskAssignment TaskAssignment { get; set; }
        public required string SubmissionUrl { get; set; }
        public string? Notes { get; set; }
        public DateTime SubmissionDate { get; set; }
        public SubmissionStatus Status { get; set; }
        public List<Review?> Reviews { get; set; } = [];
    }
}