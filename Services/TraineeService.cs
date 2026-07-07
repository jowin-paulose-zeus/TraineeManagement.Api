using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TraineeManagement.Api.Services
{
    public class TraineeService(TraineeDbContext context) : ITraineeService
    {
        private readonly TraineeDbContext _context = context;

        private static TraineeResponseRequest MapToResponse(Trainee trainee)
        {
            return new TraineeResponseRequest
            {
                Id = trainee.Id,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                Email = trainee.Email,
                TechStack = trainee.TechStack,
                Status = trainee.Status.ToString()
            };
        }
        public async Task<List<TraineeResponseRequest>> GetAllTrainees()
        {
            List<Trainee> trainees = await _context.Trainees.ToListAsync();
            return [.. trainees.Select(MapToResponse)];
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
        public async Task<TraineeResponseRequest> AddTrainee(CreateTraineeRequest request)
        {
            Trainee trainee = new()
            {
                Id = _context.Trainees.Count() + 1,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                TechStack = request.TechStack,
                Status = request.Status,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _context.Trainees.Add(trainee);
            await _context.SaveChangesAsync();
            return MapToResponse(trainee);
        }

        public async Task<bool> UpdateTraineeData(int id, UpdateTraineeRequest request)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(trainee => trainee.Id == id);

            if (trainee == null)
            {
                return false;
            }
            trainee.FirstName = request.FirstName;
            trainee.LastName = request.LastName;
            trainee.Email = request.Email;
            trainee.TechStack = request.TechStack;
            trainee.Status = request.Status;
            trainee.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTrainee(int id)
        {
            Trainee? trainee = await _context.Trainees.FirstOrDefaultAsync(trainee => trainee.Id == id);

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
            string formattedSearch = searchTerm.ToLower().Trim();
            List<Trainee> trainees = await _context.Trainees
                .Where(trainee => 
                    trainee.FirstName.ToLower().Contains(formattedSearch) || 
                    trainee.LastName.ToLower().Contains(formattedSearch) || 
                    trainee.Email.ToLower().Contains(formattedSearch) || 
                    trainee.TechStack.ToLower().Contains(formattedSearch))
                .ToListAsync();

            return [.. trainees.Select(MapToResponse)];
        }
    }
}