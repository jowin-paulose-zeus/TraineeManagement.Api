using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Data.Contracts;

namespace TrainingDirectory.Api.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    [HttpGet("{submissionId:int}")]
    public ActionResult<ProcessingProfileResponse> GetProcessingProfile(
        int submissionId)
    {
        ProcessingProfileResponse response = new()
        {
            SubmissionId = submissionId,
            RequiresChecksumValidation = true,
            RequiresVirusScan = false,
            MaxFileSizeMb = 10,
            AllowedExtensions =
            [
                ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx"
            ]
        };

        return Ok(response);
    }
}