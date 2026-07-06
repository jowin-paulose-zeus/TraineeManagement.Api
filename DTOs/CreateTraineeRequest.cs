using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Api.DTOs
{
    public class CreateTraineeRequest
    {
        [Required]
        [MaxLength(50)]
        public required string FirstName { get; set;}
        [Required]
        [MaxLength(50)]
        public required string LastName { get; set;}
        [Required]
        [EmailAddress]
        public required string Email { get; set;}
        [Required]
        public required string TechStack { get; set;}
        [Required]
        [RegularExpression("(?i)^(Active|Inactive|Completed)$",
            ErrorMessage ="Status must be Active, Inactive or Completed."),]
        public required string Status { get; set;}

    }
}