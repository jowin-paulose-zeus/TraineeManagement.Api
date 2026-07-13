using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITraineeService
    {
        Task<PagedResponse<TraineeResponseRequest>> GetTrainees(TraineeQuery query);
        Task<TraineeResponseRequest?> GetTraineeById(int id);
        Task<TraineeResponseRequest> AddTrainee(TraineeRequest request);
        Task<TraineeResponseRequest?> UpdateTraineeData(int id, TraineeRequest request);
        Task<bool> DeleteTrainee(int id);
    }
}