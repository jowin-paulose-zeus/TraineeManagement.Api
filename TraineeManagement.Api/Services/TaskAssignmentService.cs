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
    public class TaskAssignmentService(TraineeDbContext context, IDistributedCache cache, ILogger<TraineeService> logger) : ITaskAssignmentService
    {
        private readonly TraineeDbContext _context = context;
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<TraineeService> _logger = logger;
        private static TaskAssignmentResponse MapToResponse(TaskAssignment taskAssignment)
        {
            return new()
            {
                Id = taskAssignment.Id,
                TraineeId = taskAssignment.TraineeId,
                TraineeName = taskAssignment.Trainee.FirstName + " " + taskAssignment.Trainee.LastName,
                MentorId = taskAssignment.MentorId,
                MentorName = taskAssignment.Mentor.FirstName + " " + taskAssignment.Mentor.LastName,
                LearningTaskId = taskAssignment.LearningTaskId,
                LearningTaskTitle = taskAssignment.LearningTask.Title,
                AssignedDate = taskAssignment.AssignedDate,
                DueDate = taskAssignment.DueDate,
                Status = taskAssignment.Status.ToString(),
                Remarks = taskAssignment.Remarks
            };
        }

        public async Task<List<TaskAssignmentResponse>> GetTaskAssignments()
        {
            List<TaskAssignment> taskAssignments = await _context.TaskAssignments
                .Include(ta => ta.Trainee)
                .Include(ta => ta.Mentor)
                .Include(ta => ta.LearningTask)
                .AsNoTracking()
                .ToListAsync();

            if (taskAssignments is null || taskAssignments.Count == 0)
            {
                return [];
            }

            return [.. taskAssignments.Select(MapToResponse)];
        }

        public async Task<TaskAssignmentResponse?> AddTaskAssignment(TaskAssignmentRequest request)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(trainee => trainee.Id == request.TraineeId);
            if (trainee is null)
            {
                return null;

            }
            Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(mentor => mentor.Id == request.TraineeId);
            if (mentor is null)
            {
                return null;

            }
            LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(learningTask => learningTask.Id == request.TraineeId);
            if (learningTask is null)
            {
                return null;
            }
            if (request.DueDate < request.AssignedDate)
            {
                return null;
            }
            TaskAssignment assignment = new(trainee, mentor, learningTask, request.AssignedDate, request.DueDate, request.Status, request.Remarks);
            _context.TaskAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            return MapToResponse(assignment);
        }
        public async Task<TaskAssignmentResponse?> GetTaskAssignmentById(int id)
        {
            string? cacheKey = $"taskassignment:{id}";

            Func<Task<TaskAssignmentResponse?>> retrieveFromDb = async () =>
                       {
                           TaskAssignment? taskAssignment = await _context.TaskAssignments
                               .Include(task => task.Trainee)
                               .Include(task => task.Mentor)
                               .Include(task => task.LearningTask)
                               .FirstOrDefaultAsync(task => task.Id == id);

                           return taskAssignment is null ? null : MapToResponse(taskAssignment);
                       };
            return await _cache.GetOrSetAsync(cacheKey, retrieveFromDb, _logger);
        }
        public async Task<TaskAssignmentResponse?> UpdateTaskAssignment(int id, TaskAssignmentRequest request)
        {
            TaskAssignment? assignment = await _context.TaskAssignments
                .Include(ta => ta.Trainee)
                .Include(ta => ta.Mentor)
                .Include(ta => ta.LearningTask)
                .FirstOrDefaultAsync(ta => ta.Id == id);

            if (assignment is null) return null;

            Trainee? trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.Id == request.TraineeId);
            if (trainee is null) return null;

            Mentor? mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.Id == request.MentorId);
            if (mentor is null) return null;

            LearningTask? learningTask = await _context.LearningTasks
                .FirstOrDefaultAsync(lt => lt.Id == request.LearningTaskId);
            if (learningTask is null) return null;

            if (request.DueDate < request.AssignedDate) return null;

            assignment.TraineeId = request.TraineeId;
            assignment.MentorId = request.MentorId;
            assignment.LearningTaskId = request.LearningTaskId;
            assignment.AssignedDate = request.AssignedDate;
            assignment.DueDate = request.DueDate;
            assignment.Status = request.Status;
            assignment.Remarks = request.Remarks;

            _context.TaskAssignments.Update(assignment);
            await _context.SaveChangesAsync();
            string? cacheKey = $"taskassignment:{id}";
            await _cache.RemoveCacheAsync(cacheKey, _logger);
            return MapToResponse(assignment);
        }

    }
}
