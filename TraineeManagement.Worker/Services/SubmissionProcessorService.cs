using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TraineeManagement.Data.Contracts;
using TraineeManagement.Data.Configuration;
using TraineeManagement.Data.Data;
using TraineeManagement.Data.Enums;
using TraineeManagement.Worker.Interfaces;
using TraineeManagement.Data.Models;

namespace TraineeManagement.Worker.Services;

public class SubmissionProcessorService(TraineeDbContext context, ILogger<SubmissionProcessorService> logger, IOptions<FileStorageSettings> fileStorageSettings) : ISubmissionProcessorService
{
    private readonly TraineeDbContext _context = context;
    private readonly ILogger<SubmissionProcessorService> _logger = logger;
    private readonly FileStorageSettings _fileStorageSettings = fileStorageSettings.Value;

    public async Task ProcessAsync(SubmissionProcessingRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started processing SubmissionId {SubmissionId}", request.SubmissionId);

        ProcessingJob? job = await _context.ProcessingJobs
            .FirstOrDefaultAsync(
                jobprocess => jobprocess.MessageId == request.MessageId,
                cancellationToken) ?? throw new Exception("Processing job not found.");

        if (job.Status == JobStatus.Completed)
        {
            _logger.LogInformation("Skipping duplicate message {MessageId}. Job already completed.", request.MessageId);
            return;
        }

        bool alreadyProcessed = await _context.ProcessingJobs.AnyAsync(jobprocess =>
            jobprocess.Id != job.Id &&
            jobprocess.SubmissionId == request.SubmissionId &&
            jobprocess.Status == JobStatus.Completed,
        cancellationToken);
        if (alreadyProcessed)
        {
            _logger.LogInformation(
                "Submission {SubmissionId} has already been processed.",
                request.SubmissionId);

            return;
        }


        job.Status = JobStatus.Processing;
        job.StartedAt = DateTime.UtcNow;
        job.Attempts++;
        await _context.SaveChangesAsync(cancellationToken);

        Submission? submission = await _context.Submissions
            .FirstOrDefaultAsync(
                submission => submission.Id == request.SubmissionId,
                cancellationToken) ?? throw new Exception("Submission not found.");

        submission.Status = SubmissionStatus.Processing;
        await _context.SaveChangesAsync(cancellationToken);

        SubmissionFile? submissionFile = await _context.SubmissionFiles
            .FirstOrDefaultAsync(
                file => file.Id == request.FileId,
                cancellationToken);

        if (submissionFile is null)
        {
            submission.Status = SubmissionStatus.Failed;
            await _context.SaveChangesAsync(cancellationToken);
            throw new Exception("Submission file not found.");
        }

        string storageRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),_fileStorageSettings.StorageRoot));

        string filePath = Path.Combine(storageRoot,submissionFile.StoredFileName);

        if (!File.Exists(filePath))
        {
            submission.Status = SubmissionStatus.Failed;
            await _context.SaveChangesAsync(cancellationToken);

            throw new FileNotFoundException("Uploaded file not found.", filePath);
        }

        await using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);

        string calculatedChecksum = await CalculateChecksumAsync(stream, cancellationToken);

        try
        {
            if (!string.Equals(calculatedChecksum, submissionFile.Checksum, StringComparison.OrdinalIgnoreCase))
            {
                submission.Status = SubmissionStatus.Failed;
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogWarning("Checksum mismatch for SubmissionId {SubmissionId}", request.SubmissionId);
                throw new InvalidOperationException("Checksum verification failed.");
            }
        }
        catch (Exception ex)
        {
            job.Status = JobStatus.Failed;
            job.CompletedAt = DateTime.UtcNow;
            job.ErrorSummary = ex.Message;
            await _context.SaveChangesAsync(cancellationToken);

            throw;
        }

        _logger.LogInformation("Checksum verified successfully for SubmissionId {SubmissionId}", request.SubmissionId);
        submission.Status = SubmissionStatus.Completed;
        job.Status = JobStatus.Completed;
        job.CompletedAt = DateTime.UtcNow;
        job.ErrorSummary = null;
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Completed processing SubmissionId {SubmissionId}", request.SubmissionId);

    }

    private static async Task<string> CalculateChecksumAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        using SHA256 sha256 = SHA256.Create();

        byte[] hash = await sha256.ComputeHashAsync(
            stream,
            cancellationToken);

        return Convert.ToHexStringLower(hash);
    }
}