using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Interfaces
{
    public interface ILearningTaskService
    {
        Task<PagedResponse<LearningTaskResponse>> GetLearningTasks(LearningTaskQuery query);
        Task<LearningTaskResponse?> GetLearningTaskById(int id);
        Task<LearningTaskResponse> AddLearningTask(LearningTaskRequest request);
        Task<LearningTaskResponse?> UpdateLearningTaskData(int id, LearningTaskRequest request);
        Task<bool> DeleteLearningTask(int id);
    }
}