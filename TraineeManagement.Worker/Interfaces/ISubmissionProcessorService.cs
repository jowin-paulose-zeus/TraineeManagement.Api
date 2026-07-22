using TraineeManagement.Data.Contracts;

namespace TraineeManagement.Worker.Interfaces
{
    public interface ISubmissionProcessorService
    {
        Task ProcessAsync(SubmissionProcessingRequest request, CancellationToken cancellationToken);
    }
}