using TraineeManagement.Data.Enums;

namespace TraineeManagement.Data.Models
{
    public class ProcessingJob
    {
        public int Id { get; set;}
        public required Guid MessageId { get; set;}
        public required Guid CorrelationId { get; set;}
        public int SubmissionId { get; set;}
        public required Submission Submission { get; set;}
        public JobStatus Status { get; set;}
        public int Attempts { get; set;}
        public String? ErrorSummary { get; set;}
        public DateTime StartedAt { get; set;}
        public DateTime CompletedAt { get; set;}
        public DateTime CreatedAt { get; set;}
    }
}