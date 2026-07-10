using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace TraineeManagement.Api.Services
{
    public class TraineeService(TraineeDbContext context) : ITraineeService
    {
        private readonly TraineeDbContext _context = context;

        private static TraineeResponseRequest MapToResponse(Trainee trainee)
        {
            return new()
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status.ToString()
            };
        }
        public async Task<TraineePagedResponse<TraineeResponseRequest>> GetTrainees(TraineeQuery query)
        {
            IQueryable<Trainee> traineesQuery = _context.Trainees.AsQueryable();


            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                traineesQuery = traineesQuery.Where(trainee =>
                trainee.FirstName.Contains(search) ||
                trainee.LastName.Contains(search) ||
                trainee.Email.Contains(search) ||
                trainee.TechStack.Contains(search));
            }
            if (query.Status.HasValue)
            {
                traineesQuery = traineesQuery.Where(trainee =>
                    trainee.Status == query.Status.Value);
            }
            int TotalRecords = await traineesQuery.CountAsync();


            traineesQuery = traineesQuery
                .OrderBy(trainee => trainee.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            List<Trainee> trainees = await traineesQuery.ToListAsync();
            List<TraineeResponseRequest> traineeresponse = [.. trainees.Select(MapToResponse)];
            return new TraineePagedResponse<TraineeResponseRequest>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = TotalRecords,
                Data = traineeresponse
            };

        }
        public async Task<TraineeResponseRequest?> GetTraineeById(int id)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(trainee => trainee.Id == id);

            if (trainee == null)
            {
                return null;
            }

            return MapToResponse(trainee);
        }
        public async Task<TraineeResponseRequest> AddTrainee(TraineeRequest request)
        {
            Trainee trainee = new(request.FirstName, request.LastName, request.Email, request.TechStack, request.Status);
            _context.Trainees.Add(trainee);
            await _context.SaveChangesAsync();
            return MapToResponse(trainee);
        }

        public async Task<bool> UpdateTraineeData(int id, TraineeRequest request)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Id == id);

            if (trainee == null)
            {
                return false;
            }
            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.TechStack = request.TechStack;
            trainee.UpdatedDate = DateTime.UtcNow;
            _context.Trainees.Update(trainee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTrainee(int id)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Id == id);

            if (trainee == null)
            {
                return false;
            }

            _context.Trainees.Remove(trainee);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<TraineeResponseRequest>> SearchTrainees(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return [];
            }
            string formattedSearch = searchTerm.Trim();
            List<Trainee> trainees = await _context.Trainees
                .Where(trainee =>
                    trainee.FirstName.Contains(formattedSearch) ||
                    trainee.LastName.Contains(formattedSearch) ||
                    trainee.Email.Contains(formattedSearch) ||
                    trainee.TechStack.Contains(formattedSearch))
                .ToListAsync();

            return [.. trainees.Select(MapToResponse)];
        }
    }
}