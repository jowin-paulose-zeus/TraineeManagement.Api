using TraineeManagement.Data.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class SubmissionRequest
    {
        public int TaskAssignmentId { get; set; }
        public required string SubmissionUrl { get; set; }
        public string? Notes { get; set; } = string.Empty;
        public SubmissionStatus Status { get; set; }

    }
}