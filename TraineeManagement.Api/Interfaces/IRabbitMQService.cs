namespace TraineeManagement.Api.Interfaces
{
    public interface IRabbitMQService
    {
        Task<bool> RabbitMQPublish<T> (T message);
    }
}