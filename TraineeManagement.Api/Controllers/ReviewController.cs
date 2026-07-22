using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]

    public class ReviewController(IReviewService reviewService, ILogger<ReviewController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddReview(ReviewRequest request)
        {
            logger.LogInformation("Creating review for SubmissionId: {SubmissionId}", request.SubmissionId);
            
            ReviewResponse? review = await reviewService.AddReview(request);

            if (review is null)
            {
                logger.LogWarning("Review creation failed.");
                return BadRequest("Invalid trainee, task assignment, or review details.");
            }

            logger.LogInformation("Review created successfully. Id: {Id}", review.Id);
            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            logger.LogInformation("Fetching all reviews.");
            
            List<ReviewResponse> reviews = await reviewService.GetReviews();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            logger.LogInformation("Fetching review with Id: {Id}", id);

            ReviewResponse? review = await reviewService.GetReviewById(id);

            if (review is null)
            {
                logger.LogWarning("Review not found. Id: {Id}", id);
                return NotFound();
            }

            return Ok(review);
        }
    }
}
