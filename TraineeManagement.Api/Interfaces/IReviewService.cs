using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewResponse>> GetReviews();
        Task<ReviewResponse?> GetReviewById(int id);
        Task<ReviewResponse?> AddReview(ReviewRequest request);
    }
}