using TraineeManagement.Api.DTOs;
using TraineeManagement.Data.Models;
using TraineeManagement.Data.Data;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace TraineeManagement.Api.Services
{
    public class MentorService(TraineeDbContext context) : IMentorService
    {
        private readonly TraineeDbContext _context = context;

        private static MentorResponse MapToResponse(Mentor mentor)
        {
            return new()
            {
                Id = mentor.Id,
                FirstName = mentor.FirstName,
                LastName = mentor.LastName,
                Email = mentor.Email,
                Expertise = mentor.Expertise,
                Status = mentor.Status.ToString()
            };
        }
        public async Task<PagedResponse<MentorResponse>> GetMentors(MentorQuery query)
        {
            IQueryable<Mentor> mentorsQuery = _context.Mentors.AsQueryable();


            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                mentorsQuery = mentorsQuery.Where(mentor =>
                mentor.FirstName.Contains(search) ||
                mentor.LastName.Contains(search) ||
                mentor.Email.Contains(search) ||
                mentor.Expertise.Contains(search));
            }
            if (query.Status.HasValue)
            {
                mentorsQuery = mentorsQuery.Where(mentor =>
                    mentor.Status == query.Status.Value);
            }
            int TotalRecords = await mentorsQuery.CountAsync();

            if (query.PageNumber < 1) { query.PageNumber = 1; }
            if (query.PageSize < 1 || query.PageSize > 50) { query.PageSize = 10; }
            mentorsQuery = mentorsQuery
                .OrderBy(trainee => trainee.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            List<Mentor> mentors = await mentorsQuery.ToListAsync();
            List<MentorResponse> response = [.. mentors.Select(MapToResponse)];
            return new PagedResponse<MentorResponse>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = TotalRecords,
                Data = response
            };

        }
        public async Task<MentorResponse?> GetMentorById(int id)
        {
            Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(mentor => mentor.Id == id);

            if (mentor == null)
            {
                return null;
            }

            return MapToResponse(mentor);
        }
        public async Task<MentorResponse> AddMentor(MentorRequest request)
        {
            Mentor mentor = new(request.FirstName, request.LastName, request.Email, request.Expertise, request.Status)
            {
                CreatedDate = DateTime.UtcNow
            };
            _context.Mentors.Add(mentor);
            await _context.SaveChangesAsync();
            return MapToResponse(mentor);
        }

        public async Task<MentorResponse?> UpdateMentorData(int id, MentorRequest request)
        {
            Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(mentor => mentor.Id == id);

            if (mentor == null)
            {
                return null;
            }
            mentor.FirstName = request.FirstName;
            mentor.LastName = request.LastName;
            mentor.Email = request.Email;
            mentor.Expertise = request.Expertise;
            mentor.Status = request.Status;
            mentor.UpdatedDate = DateTime.UtcNow;
            _context.Mentors.Update(mentor);
            await _context.SaveChangesAsync();
            return MapToResponse(mentor);
        }
        public async Task<bool> DeleteMentor(int id)
        {
            Mentor? mentor = await _context.Mentors.FirstOrDefaultAsync(mentor => mentor.Id == id);

            if (mentor == null)
            {
                return false;
            }

            _context.Mentors.Remove(mentor);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}