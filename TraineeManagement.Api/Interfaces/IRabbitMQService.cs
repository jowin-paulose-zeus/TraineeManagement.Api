namespace TraineeManagement.Api.Interfaces
{
    public interface IRabbitMQService
    {
        Task PublishSubmissionAsync<T> (T message);
    }
}