using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.Clients;
using TraineeManagement.Data.Contracts;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/directory-profile")]
public class DirectoryProfileController(ITrainingDirectoryClient trainingDirectoryClient) : ControllerBase
{
    private readonly ITrainingDirectoryClient _trainingDirectoryClient =
        trainingDirectoryClient;
    [HttpGet("{submissionId:int}")]
    public async Task<IActionResult> GetProcessingProfile(
        int submissionId,
        CancellationToken cancellationToken)
    {
        try
        {
            string correlationId = Guid.NewGuid().ToString();

            ProcessingProfileResponse profile = await _trainingDirectoryClient.GetProcessingProfileAsync(
                submissionId,
                correlationId,
                cancellationToken);

            return Ok(profile);
        }
        catch (Exception)
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