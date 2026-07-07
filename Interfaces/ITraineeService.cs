using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Services
{
    public interface ITraineeService
    {
        List<TraineeResponseRequest> GetAllTrainees();
        TraineeResponseRequest? GetTraineeById(int id);
        TraineeResponseRequest AddTrainee(CreateTraineeRequest request);
        bool UpdateTraineeData(int id, UpdateTraineeRequest request);
        bool DeleteTrainee(int id);
    }
}