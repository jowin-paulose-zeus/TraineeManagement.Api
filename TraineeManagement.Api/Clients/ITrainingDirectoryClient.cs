using TraineeManagement.Data.Contracts;

namespace TraineeManagement.Api.Clients;

public interface ITrainingDirectoryClient
{
    Task<ProcessingProfileResponse> GetProcessingProfileAsync(
        int submissionId,
        string correlationId,
        CancellationToken cancellationToken);
}