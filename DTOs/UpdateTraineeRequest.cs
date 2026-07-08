using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class UpdateTraineeRequest
    {

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MaxLength(100)]
        public required string TechStack { get; set; }

        [EnumDataType(typeof(TraineeStatus), ErrorMessage = "Invalid trainee status value.Valid values are: 1(Active), 2(Inactive), 3(Completed)")]
        public required TraineeStatus Status { get; set; }

    }
}