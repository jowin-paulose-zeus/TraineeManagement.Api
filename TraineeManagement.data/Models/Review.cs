using System.Diagnostics.CodeAnalysis;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Data.Models
{
    public class Review
    {
        public Review() { }
        [SetsRequiredMembers]
        public Review(Submission submission,Mentor mentor,string feedback,ReviewStatus status, int? score = null)
        {
            SubmissionId = submission.Id;
            Submission = submission;
            MentorId = mentor.Id;
            Mentor = mentor;
            Feedback = feedback;
            Score = score;
            ReviewedDate = DateTime.UtcNow;
            Status = status;
        }

        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public required Submission Submission { get; set; }
        public int MentorId { get; set; }
        public required Mentor Mentor { get; set; }
        public required string Feedback { get; set; }
        public int? Score { get; set; }
        public DateTime ReviewedDate { get; set; }
        public ReviewStatus Status { get; set; }
    }
}