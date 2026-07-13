using System.ComponentModel.DataAnnotations;
using TraineeManagement.Api.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class MentorRequest
    {

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MaxLength(100)]
        public required string Expertise { get; set; }

        [EnumDataType(typeof(MentorStatus), ErrorMessage = "Invalid mentor status value.Valid values are: 1(Active), 2(Inactive)")]

        public required MentorStatus Status { get; set; }

    }
}
