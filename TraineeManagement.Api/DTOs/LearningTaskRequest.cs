using System.ComponentModel.DataAnnotations;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class LearningTaskRequest
    {

        [MaxLength(50)]
        public required string Title { get; set; }

        [MaxLength(100)]
        public required string Description { get; set; }

        [MaxLength(50)]
        public required string ExpectedTechStack { get; set; }

        [EnumDataType(typeof(LearningTaskStatus), ErrorMessage = "Invalid LearningTask status value.")]

        public required LearningTaskStatus Status { get; set; }
        public required DateTime DueDate { get; set; }

    }
}
