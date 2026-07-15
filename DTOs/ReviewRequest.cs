using TraineeManagement.Api.Enums;
namespace TraineeManagement.Api.DTOs
{
    public class ReviewRequest
    {
        public required int SubmissionId { get; set; }
        public required int MentorId { get; set; }
        public required string Feedback { get; set; }
        public int? Score { get; set; } = null;
        public ReviewStatus Status { get; set; }

    }
}