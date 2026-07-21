using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface IMentorService
    {
        Task<PagedResponse<MentorResponse>> GetMentors(MentorQuery query);
        Task<MentorResponse?> GetMentorById(int id);
        Task<MentorResponse> AddMentor(MentorRequest request);
        Task<MentorResponse?> UpdateMentorData(int id, MentorRequest request);
        Task<bool> DeleteMentor(int id);
    }
}