namespace TraineeManagement.Api.DTOs
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public required string MentorName { get; set; }
        public required string Feedback { get; set; }
        public required DateTime ReviewedDate { get; set; }
        public required String Status { get; set; }
        public int? Score { get; set; } = null;

    }
}