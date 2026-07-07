using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Services
{
    public interface ITraineeService
    {
        Task<List<TraineeResponseRequest>> GetAllTrainees();
        Task<TraineeResponseRequest?> GetTraineeById(int id);
        Task<TraineeResponseRequest> AddTrainee(CreateTraineeRequest request);
        Task<bool> UpdateTraineeData(int id, UpdateTraineeRequest request);
        Task<bool> DeleteTrainee(int id);
        Task<List<TraineeResponseRequest>> SearchTrainees(string searchTerm);
    }
}