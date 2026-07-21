using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]

    public class SubmissionController(ISubmissionService submissionService, ILogger<SubmissionController> logger) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddSubmission(SubmissionRequest request)
        {
            logger.LogInformation("Creating submission for TaskAssignmentId: {TaskAssignmentId}", request.TaskAssignmentId);
            
            SubmissionResponse? submission = await submissionService.AddSubmission(request);

            if (submission == null)
            {
                logger.LogWarning("Submission creation failed.");
                return BadRequest("Invalid trainee, task assignment, or submission details.");
            }

            logger.LogInformation("Submission created successfully. Id: {Id}", submission.Id);
            return CreatedAtAction(nameof(GetSubmissionById), new { id = submission.Id }, submission);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubmissions()
        {
            logger.LogInformation("Fetching all submissions.");
            
            List<SubmissionResponse> submissions = await submissionService.GetSubmissions();
            return Ok(submissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmissionById(int id)
        {
            logger.LogInformation("Fetching submission with Id: {Id}", id);

            SubmissionResponse? submission = await submissionService.GetSubmissionById(id);

            if (submission == null)
            {
                logger.LogWarning("Submission not found. Id: {Id}", id);
                return NotFound();
            }

            return Ok(submission);
        }
    }
}
