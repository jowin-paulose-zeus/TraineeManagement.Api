namespace TraineeManagement.Api.DTOs
{
    public class LearningTaskResponse
    {

        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string ExpectedTechStack { get; set; }
        public required string Status { get; set; }
        public required DateTime DueDate { get; set; }

    }
}