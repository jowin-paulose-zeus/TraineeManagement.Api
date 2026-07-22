using TraineeManagement.Api.DTOs;
using TraineeManagement.Data.Models;
using TraineeManagement.Data.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace TraineeManagement.Api.Services
{
    public class LearningTaskService(TraineeDbContext context) : ILearningTaskService
    {
        private readonly TraineeDbContext _context = context;

        private static LearningTaskResponse MapToResponse(LearningTask learningtask)
        {
            return new()
            {
                Id = learningtask.Id,
                Title = learningtask.Title,
                Description = learningtask.Description,
                ExpectedTechStack = learningtask.ExpectedTechStack,
                Status = learningtask.Status.ToString(),
                DueDate = learningtask.DueDate
            };
        }
        public async Task<PagedResponse<LearningTaskResponse>> GetLearningTasks(LearningTaskQuery query)
        {
            IQueryable<LearningTask> learningtasksQuery = _context.LearningTasks.AsQueryable();


            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                learningtasksQuery = learningtasksQuery.Where(learningtask =>
                learningtask.Title.Contains(search) ||
                learningtask.Description.Contains(search) ||
                learningtask.ExpectedTechStack.Contains(search));
            }
            if (query.Status.HasValue)
            {
                learningtasksQuery = learningtasksQuery.Where(learningtask =>
                    learningtask.Status == query.Status.Value);
            }
            int TotalRecords = await learningtasksQuery.CountAsync();

            if (query.PageNumber < 1) { query.PageNumber = 1; }
            if (query.PageSize < 1 || query.PageSize > 50) { query.PageSize = 10; }
            learningtasksQuery = learningtasksQuery
                .OrderBy(trainee => trainee.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            List<LearningTask> learningtasks = await learningtasksQuery.ToListAsync();
            List<LearningTaskResponse> response = [.. learningtasks.Select(MapToResponse)];
            return new PagedResponse<LearningTaskResponse>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = TotalRecords,
                Data = response
            };

        }
        public async Task<LearningTaskResponse?> GetLearningTaskById(int id)
        {
            LearningTask? learningtask = await _context.LearningTasks.FirstOrDefaultAsync(learningtask => learningtask.Id == id);

            if (learningtask is null)
            {
                return null;
            }

            return MapToResponse(learningtask);
        }
        public async Task<LearningTaskResponse> AddLearningTask(LearningTaskRequest request)
        {
            LearningTask learningtask = new(request.Title, request.Description, request.ExpectedTechStack,request.DueDate, request.Status)
            {
                CreatedDate = DateTime.UtcNow
            };
            _context.LearningTasks.Add(learningtask);
            await _context.SaveChangesAsync();
            return MapToResponse(learningtask);
        }

        public async Task<LearningTaskResponse?> UpdateLearningTaskData(int id, LearningTaskRequest request)
        {
            LearningTask? learningtask = await _context.LearningTasks.FirstOrDefaultAsync(learningtask => learningtask.Id == id);

            if (learningtask is null)
            {
                return null;
            }
            learningtask.Title = request.Title;
            learningtask.Description = request.Description;
            learningtask.ExpectedTechStack = request.ExpectedTechStack;
            learningtask.DueDate = request.DueDate;
            learningtask.Status = request.Status;
            learningtask.UpdatedDate = DateTime.UtcNow;
            _context.LearningTasks.Update(learningtask);
            await _context.SaveChangesAsync();
            return MapToResponse(learningtask);
        }
        public async Task<bool> DeleteLearningTask(int id)
        {
            LearningTask? learningtask = await _context.LearningTasks.FirstOrDefaultAsync(learningtask => learningtask.Id == id);

            if (learningtask is null)
            {
                return false;
            }

            _context.LearningTasks.Remove(learningtask);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}