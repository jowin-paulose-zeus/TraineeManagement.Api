using TraineeManagement.Api.DTOs;
using TraineeManagement.Data.Models;
using TraineeManagement.Data.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TraineeManagement.Api.Helpers;

namespace TraineeManagement.Api.Services
{
    public class SubmissionService(TraineeDbContext context, IDistributedCache cache, ILogger<TraineeService> logger) : ISubmissionService
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<TraineeService> _logger = logger;
        private static SubmissionResponse MapToResponse(Submission submission)
        {
            return new()
            {
                Id = submission.Id,
                TaskAssignmentId = submission.TaskAssignmentId,
                TraineeName = $"{submission.TaskAssignment?.Trainee?.FirstName} {submission.TaskAssignment?.Trainee?.LastName}".Trim(),
                TaskTitle = submission.TaskAssignment?.LearningTask?.Title ?? string.Empty,
                SubmissionUrl = submission.SubmissionUrl,
                SubmissionDate = submission.SubmissionDate,
                Status = submission.Status.ToString(),
                Notes = submission.Notes
            };
        }

        public async Task<List<SubmissionResponse>> GetSubmissions()
        {
            List<Submission> submissions = await context.Submissions
                .Include(submission => submission.TaskAssignment)
                    .ThenInclude(taskassignment => taskassignment.Trainee)
                .Include(submission => submission.TaskAssignment)
                    .ThenInclude(taskassignment => taskassignment.LearningTask)
                .AsNoTracking()
                .ToListAsync();

            return [.. submissions.Select(MapToResponse)];
        }

        public async Task<SubmissionResponse?> GetSubmissionById(int id)
        {
            string? cacheKey = $"Submission:{id}";

            async Task<SubmissionResponse?> retrieveFromDb()
            {
                Submission? submission = await context.Submissions
                    .Include(currentSubmission => currentSubmission.TaskAssignment)
                    .ThenInclude(ta => ta.LearningTask)
                    .FirstOrDefaultAsync(s => s.Id == id);
                return submission is null ? null : MapToResponse(submission);
            }
            return await _cache.GetOrSetAsync(cacheKey, retrieveFromDb, _logger);
        }

        public async Task<SubmissionResponse?> AddSubmission(SubmissionRequest request)
        {

            bool submissionExists = await context.Submissions
                .AnyAsync(s => s.TaskAssignmentId == request.TaskAssignmentId);

            if (submissionExists) return null;
            TaskAssignment? taskAssignment = await context.TaskAssignments
                .Include(taskassignment => taskassignment.Trainee)
                .Include(taskassignment => taskassignment.LearningTask)
                .FirstOrDefaultAsync(taskassignment => taskassignment.Id == request.TaskAssignmentId);

            if (taskAssignment is null) return null;

            Submission submission = new(taskAssignment, request.SubmissionUrl, request.Status, request.Notes);

            context.Submissions.Add(submission);
            await context.SaveChangesAsync();

            return MapToResponse(submission);
        }
    }
}
