using TraineeManagement.Api.DTOs;
using Microsoft.Extensions.Options;
using TraineeManagement.Data.Configuration;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Data.Data;
using TraineeManagement.Data.Models;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.Enums;
using System.Security.Cryptography;
using TraineeManagement.Contracts.Contracts;

namespace TraineeManagement.Api.Services
{
    public class LocalFileStorageService(TraineeDbContext context,
        IOptions<FileStorageSettings> fileStorageSettings,
        ILogger<Trainee> logger,
        IRabbitMQService rabbitMQService) : IFileStorageService
    {
        private readonly TraineeDbContext _context = context;
        private readonly string _storageRoot = InitializeStorage(fileStorageSettings.Value.StorageRoot);
        private readonly IOptions<FileStorageSettings> _fileStorageSettings = fileStorageSettings;
        private readonly ILogger _logger = logger;
        private readonly IRabbitMQService _rabbitMQService = rabbitMQService;

        private static string InitializeStorage(string storagePath)
        {
            string root = Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    storagePath));

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            return root;
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(_storageRoot, storedFileName);
            using FileStream stream = new(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return storedFileName;
        }

        public async Task<Stream> OpenReadAsync(string fileName)
        {
            string filePath = Path.Combine(_storageRoot, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.");
            }
            return await Task.FromResult<Stream>(
            new FileStream(filePath, FileMode.Open, FileAccess.Read));
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            string filePath = Path.Combine(_storageRoot, fileName);
            return await Task.FromResult(File.Exists(filePath));
        }

        public async Task<DownloadSubmissionFileResponse> DownloadAsync(int id)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    subFile => subFile.Id == id) ?? throw new KeyNotFoundException($"Submission file {id} was not found.");
            Stream? stream = await this.OpenReadAsync(submissionFile.StoredFileName);

            return new DownloadSubmissionFileResponse
            {
                Stream = stream,
                ContentType = submissionFile.ContentType,
                FileName = submissionFile.OriginalFileName
            };
        }
        public async Task DeleteAsync(int id)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                    subFile => subFile.Id == id) ?? throw new KeyNotFoundException($"Submission file {id} was not found.");
            string filePath = Path.Combine(_storageRoot, submissionFile.StoredFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await Task.CompletedTask;

            _context.SubmissionFiles.Remove(submissionFile);

            await _context.SaveChangesAsync();

        }
        public async Task<FileStorageResponse> Upload(int submissionId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogError("File is missing or empty.");
                    throw new ArgumentException("File is missing or empty.");
                }
            }

            if (file.Length > _fileStorageSettings.Value.MaxFileSize)
            {
                _logger.LogError($"File size exceeds the limit of {_fileStorageSettings.Value.MaxFileSize / 1024 / 1024} MB.");
                throw new ArgumentException($"File size exceeds the limit of {_fileStorageSettings.Value.MaxFileSize / 1024 / 1024} MB.");
            }

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_fileStorageSettings.Value.AllowedExtentions.Contains(extension))
            {
                _logger.LogError("File type extention is not allowed");
                throw new ArgumentException("File type extention is not allowed");
            }

            Submission? submission = await _context.Submissions
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null)
            {
                _logger.LogError("Submission record not found.");
                throw new ArgumentException("Submission record not found.");
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
                storageFileName = await this.SaveAsync(file);
            }
            catch (Exception)
            {
                _logger.LogError(StatusCodes.Status500InternalServerError, "Error saving file to persistent storage.");
                throw new ArgumentException("Error saving file to persistent storage.");
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

            SubmissionProcessingRequest message = new()
            {
                MessageId = Guid.NewGuid(),
                CorrelationId = Guid.NewGuid(),
                SubmissionId = submission.Id,
                FileId = dbFile.Id,
                RequestedAt = DateTime.UtcNow,
                ContractVersion = "1.0"
            };
            ProcessingJob job = new()
            {
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                SubmissionId = submission.Id,
                Submission = submission,
                Status = JobStatus.Queued,
                Attempts = 0,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProcessingJobs.Add(job);

            await _context.SaveChangesAsync();

            try
            {
                await _rabbitMQService.PublishSubmissionAsync(message);

                _logger.LogInformation(
                    "RabbitMQ publish successful. MessageId: {MessageId}, CorrelationId: {CorrelationId}, SubmissionId: {SubmissionId}",
                    message.MessageId,
                    message.CorrelationId,
                    message.SubmissionId);
            }
            catch (Exception ex)
            {
                job.Status = JobStatus.Failed;
                job.ErrorSummary = "Failed to publish message.";
                job.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                _logger.LogError(
                    ex,
                    "Failed to queue submission. SubmissionId: {SubmissionId}",
                    message.SubmissionId);

                throw new InvalidOperationException("Unable to queue submission for processing", ex);
            }

            _logger.LogInformation(
                "Published submission message. MessageId: {MessageId}, CorrelationId: {CorrelationId}, SubmissionId: {SubmissionId}",
                message.MessageId,
                message.CorrelationId,
                message.SubmissionId);

            return new FileStorageResponse()
            {
                Id = dbFile.Id,
                SubmissionId = dbFile.SubmissionId,
                ContentType = dbFile.ContentType,
                OriginalFileName = dbFile.OriginalFileName,
                Size = dbFile.Size,
                TrackingId = message.CorrelationId
            };

        }
    }
}