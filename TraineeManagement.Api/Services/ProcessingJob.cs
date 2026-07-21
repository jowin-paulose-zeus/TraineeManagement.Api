using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Data.Data;
using TraineeManagement.Data.Models;

namespace TraineeManagement.Api.Services;

public class ProcessingJobService(TraineeDbContext context) : IProcessingJobService
{
    private readonly TraineeDbContext _context = context;

    public async Task<ProcessingJobResponse> GetByIdAsync(int id)
    {
        ProcessingJob job = await _context.ProcessingJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new KeyNotFoundException(
                $"Processing job {id} was not found.");

        return new ProcessingJobResponse
        {
            Id = job.Id,
            MessageId = job.MessageId,
            CorrelationId = job.CorrelationId,
            SubmissionId = job.SubmissionId,
            Status = job.Status.ToString(),
            Attempts = job.Attempts,
            ErrorSummary = job.ErrorSummary,
            StartedAt = job.StartedAt,
            CompletedAt = job.CompletedAt,
            CreatedAt = job.CreatedAt
        };
    }
}