using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class SubmissionResponse
    {
        public int Id { get; set; }
        public int TaskAssignmentId { get; set; }
        public required string TraineeName { get; set; }
        public required string TaskTitle { get; set; }
        public required string SubmissionUrl { get; set; }
        public required DateTime SubmissionDate { get; set; }
        public required string Status { get; set; }
        public string? Notes { get; set; } = string.Empty;
    }
}