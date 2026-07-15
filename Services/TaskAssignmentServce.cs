using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Services
{
    public class TaskAssignmentService(TraineeDbContext context) : ITaskAssignmentService
    {
        private readonly TraineeDbContext _context = context;
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

            if (taskAssignments == null || taskAssignments.Count == 0)
            {
                return [];
            }

            return [.. taskAssignments.Select(MapToResponse)];
        }

        public async Task<TaskAssignmentResponse?> AddTaskAssignment(TaskAssignmentRequest request)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(trainee => trainee.Id == request.TraineeId);
            if (trainee == null)
            {
                return null;

            }
            Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(mentor => mentor.Id == request.TraineeId);
            if (mentor == null)
            {
                return null;

            }
            LearningTask? learningTask = await _context.LearningTasks.FirstOrDefaultAsync(learningTask => learningTask.Id == request.TraineeId);
            if (learningTask == null)
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
            TaskAssignment? taskAssignment = await _context.TaskAssignments
            .Include(trainee => trainee.Trainee)
            .Include(mentor => mentor.Mentor)
            .Include(learningtask => learningtask.LearningTask)
            .FirstOrDefaultAsync(taskAssignment => taskAssignment.Id == id);

            if (taskAssignment == null)
            {
                return null;
            }
            return MapToResponse(taskAssignment);
        }
        public async Task<TaskAssignmentResponse?> UpdateTaskAssignment(int id, TaskAssignmentRequest request)
        {
            TaskAssignment? assignment = await _context.TaskAssignments
                .Include(ta => ta.Trainee)
                .Include(ta => ta.Mentor)
                .Include(ta => ta.LearningTask)
                .FirstOrDefaultAsync(ta => ta.Id == id);

            if (assignment == null) return null;

            Trainee? trainee = await _context.Trainees
                .FirstOrDefaultAsync(t => t.Id == request.TraineeId);
            if (trainee == null) return null;

            Mentor? mentor = await _context.Mentors
                .FirstOrDefaultAsync(m => m.Id == request.MentorId);
            if (mentor == null) return null;

            LearningTask? learningTask = await _context.LearningTasks
                .FirstOrDefaultAsync(lt => lt.Id == request.LearningTaskId);
            if (learningTask == null) return null;

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

            return MapToResponse(assignment);
        }

    }
}
