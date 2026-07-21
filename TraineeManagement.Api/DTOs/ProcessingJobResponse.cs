using TraineeManagement.Data.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class ProcessingJobResponse
    {
        public int Id { get; set; }
        public Guid MessageId { get; set; }
        public Guid CorrelationId { get; set; }
        public int SubmissionId { get; set; }
        public String? Status { get; set; }
        public int Attempts { get; set; }
        public string? ErrorSummary { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}