using TraineeManagement.Contracts.Contracts;

namespace TraineeManagement.Worker.Interfaces
{
    public interface ISubmissionProcessorService
    {
        Task ProcessAsync(SubmissionProcessingRequest request, CancellationToken cancellationToken);
    }
}