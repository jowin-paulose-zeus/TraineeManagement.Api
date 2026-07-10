using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITraineeService
    {
        Task<TraineePagedResponse<TraineeResponseRequest>> GetTrainees(TraineeQuery query);
        Task<TraineeResponseRequest?> GetTraineeById(int id);
        Task<TraineeResponseRequest> AddTrainee(TraineeRequest request);
        Task<bool> UpdateTraineeData(int id, TraineeRequest request);
        Task<bool> DeleteTrainee(int id);
        Task<List<TraineeResponseRequest>> SearchTrainees(string searchTerm);
    }
}