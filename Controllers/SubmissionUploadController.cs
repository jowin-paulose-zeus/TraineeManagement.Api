using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Configuration;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/submissionfile")]
    public class SubmissionUploadController(
        TraineeDbContext context,
        IFileStorageService storageService,
        IOptions<FileStorageSettings> settings) : ControllerBase
    {
        private readonly TraineeDbContext _context = context;
        private readonly IFileStorageService _storageService = storageService;
        private readonly FileStorageSettings _settings = settings.Value;

        [HttpPost("{submissionId}/files")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile(int submissionId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is missing or empty.");
            }

            if (file.Length > _settings.MaxFileSize)
            {
                return BadRequest($"File size exceeds the limit of {_settings.MaxFileSize / 1024 / 1024} MB.");
            }

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_settings.AllowedExtentions.Contains(extension))
            {
                return BadRequest("File type extension is not allowed.");
            }

            Submission? submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null)
            {
                return NotFound("Submission record not found.");
            }

            string checksum;
            using (SHA256 sha256 = SHA256.Create())
            using (Stream? stream = file.OpenReadStream())
            {
                byte[] hashBytes = await sha256.ComputeHashAsync(stream);
                checksum = Convert.ToHexStringLower(hashBytes);
            }

            string storageFileName;
            try
            {
                storageFileName = await _storageService.SaveAsync(file);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving file to persistent storage.");
            }


            SubmissionFile dbFile = new()
            {
                SubmissionId = submissionId,
                Submission = submission,
                OriginalFileName = Path.GetFileName(file.FileName),
                StoredFileName = storageFileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Checksum = checksum,
            };

            _context.SubmissionFiles.Add(dbFile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(UploadFile), new { id = dbFile.Id }, new
            {
                dbFile.Id,
                dbFile.OriginalFileName,
                dbFile.Size,
                dbFile.ContentType
            });

        }
        [HttpGet("submission-files/{id:int}/download")]
        public async Task<IActionResult> Download(int id)
        {
            DownloadSubmissionFileResponse? response = await _storageService.DownloadAsync(id);

            return File(
                response.Stream,
                response.ContentType,
                response.FileName);
        }
        [HttpDelete("submission-files/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _storageService.DeleteAsync(id);
            return NoContent();
        }
    }
}