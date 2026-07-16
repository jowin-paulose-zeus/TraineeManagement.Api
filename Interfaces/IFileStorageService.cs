using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile formFile);
        Task<Stream> OpenReadAsync(string fileName);
        Task<bool> ExistsAsync(string fileName);
        Task DeleteAsync(int id);
        Task<DownloadSubmissionFileResponse> DownloadAsync(int id);
    }
}