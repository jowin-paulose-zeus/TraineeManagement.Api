using TraineeManagement.Api.DTOs;
using TraineeManagement.Data.Models;
using TraineeManagement.Data.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class ReviewService(TraineeDbContext context) : IReviewService
    {
        private static ReviewResponse MapToResponse(Review review)
        {
            return new()
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                MentorName = review.Mentor.FirstName + " " + review.Mentor.LastName,
                Feedback = review.Feedback,
                ReviewedDate = review.ReviewedDate,
                Score = review.Score,
                Status = review.Status.ToString()
            };
        }

        public async Task<List<ReviewResponse>> GetReviews()
        {
            List<Review> reviews = await context.Reviews
                .Include(reviews => reviews.Mentor)
                .Include(reviews => reviews.Submission)
                    .ThenInclude(submission => submission.TaskAssignment)
                .AsNoTracking()
                .ToListAsync();

            return reviews.Select(MapToResponse).ToList();
        }

        public async Task<ReviewResponse?> GetReviewById(int id)
        {
            Review? review = await context.Reviews
                .Include(reviews => reviews.Mentor)
                .Include(review => review.Submission)
                    .ThenInclude(submission => submission.TaskAssignment)
                .AsNoTracking()
                .FirstOrDefaultAsync(review => review.Id == id);

            if (review == null) return null;

            return MapToResponse(review);
        }

        public async Task<ReviewResponse?> AddReview(ReviewRequest request)
        {

            bool reviewByMentorExists = await context.Reviews
                .AnyAsync(r => r.SubmissionId == request.SubmissionId && r.MentorId == request.MentorId);

            if (reviewByMentorExists) return null;

            Submission? submission = await context.Submissions
                .Include(s => s.TaskAssignment)
                    .ThenInclude(ta => ta.Mentor)
                .FirstOrDefaultAsync(s => s.Id == request.SubmissionId);

            if (submission == null) return null;

            Review review = new(submission, submission.TaskAssignment.Mentor, request.Feedback, request.Status, request.Score);

            context.Reviews.Add(review);
            await context.SaveChangesAsync();

            return MapToResponse(review);
        }

    }
}
