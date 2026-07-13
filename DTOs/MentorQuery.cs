using System.Diagnostics.Contracts;
using TraineeManagement.Api.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class MentorQuery
    {
        public int PageNumber { get; set; } =1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public MentorStatus? Status { get; set; }
    }
}