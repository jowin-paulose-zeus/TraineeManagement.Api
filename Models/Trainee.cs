using System.ComponentModel.DataAnnotations;

namespace TraineeManagement.Api.Models
{
    public class Trainee
    {
        public required int Id { get; set; }
        public required string FirstName { get; set; }
        public string LastName { get; set; }
        public required string Email { get; set; }
        public required string TechStack { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}