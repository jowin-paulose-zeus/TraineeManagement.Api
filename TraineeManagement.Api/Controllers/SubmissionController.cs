using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TraineeManagement.Api.Clients;
using TraineeManagement.Data.Contracts;


namespace TraineeManagement.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]

    public class SubmissionController(ISubmissionService submissionService,
        ILogger<SubmissionController> logger,
        ITrainingDirectoryClient trainingDirectoryClient) : ControllerBase
    {
        private readonly ITrainingDirectoryClient _trainingDirectoryClient = trainingDirectoryClient;
        [HttpPost]
        public async Task<IActionResult> AddSubmission(SubmissionRequest request)
        {
            logger.LogInformation("Creating submission for TaskAssignmentId: {TaskAssignmentId}", request.TaskAssignmentId);

            SubmissionResponse? submission = await submissionService.AddSubmission(request);

            if (submission is null)
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

            if (submission is null)
            {
                logger.LogWarning("Submission not found. Id: {Id}", id);
                return NotFound();
            }

            return Ok(submission);
        }
        [HttpGet("{submissionId:int}/processing-profile")]
        public async Task<IActionResult> GetProcessingProfile(
            int submissionId,
            CancellationToken cancellationToken)
        {
            try
            {
                string correlationId = Guid.NewGuid().ToString();

                var profile = await _trainingDirectoryClient.GetProcessingProfileAsync(
                    submissionId,
                    correlationId,
                    cancellationToken);

                return Ok(profile);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new
                    {
                        Message = "Training Directory service is currently unavailable."
                    });
            }
        }
    }
}
