using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ISubmissionService
    {
        Task<List<SubmissionResponse>> GetSubmissions();
        Task<SubmissionResponse?> GetSubmissionById(int id);
        Task<SubmissionResponse?> AddSubmission(SubmissionRequest request);
    }
}