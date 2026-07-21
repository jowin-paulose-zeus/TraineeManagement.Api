using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface ITaskAssignmentService
    {
        Task<List<TaskAssignmentResponse>> GetTaskAssignments();
        Task<TaskAssignmentResponse?> GetTaskAssignmentById(int id);
        Task<TaskAssignmentResponse?> AddTaskAssignment(TaskAssignmentRequest request);
        Task<TaskAssignmentResponse?> UpdateTaskAssignment(int id, TaskAssignmentRequest request);
    }
}