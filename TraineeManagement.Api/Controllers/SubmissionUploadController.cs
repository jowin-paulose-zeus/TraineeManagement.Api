using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Data.Data;
using TraineeManagement.Data.Configuration;
using TraineeManagement.Data.Models;
using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/submissionfile")]
    public class SubmissionUploadController(
        TraineeDbContext context,
        IFileStorageService storageService,
        IOptions<FileStorageSettings> settings,
        ILogger<Trainee> logger) : ControllerBase
    {
        private readonly TraineeDbContext _context = context;
        private readonly IFileStorageService _storageService = storageService;
        private readonly FileStorageSettings _settings = settings.Value;
        private readonly ILogger _logger = logger;

        [HttpPost("{submissionId}/files")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(int submissionId, IFormFile file)
        {
            try
            {
                FileStorageResponse response = await _storageService.Upload(submissionId, file);

                return Accepted(new
                {
                    TrackingId = response.TrackingId,
                    SubmissionId = response.SubmissionId,
                    Status = "Queued"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed during file upload for submission {SubmissionId}.", submissionId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error uploading file for submission {SubmissionId}.", submissionId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing your request." });
            }
        }
        [HttpGet("{id:int}/download")]
        public async Task<IActionResult> Download(int id)
        {
            DownloadSubmissionFileResponse? response = await _storageService.DownloadAsync(id);

            return File(
                response.Stream,
                response.ContentType,
                response.FileName);
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _storageService.DeleteAsync(id);
            return NoContent();
        }
    }
}