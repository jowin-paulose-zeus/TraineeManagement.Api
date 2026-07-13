using System.Diagnostics.Contracts;
using TraineeManagement.Api.Enums;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.DTOs
{
    public class LearningTaskQuery
    {
        public int PageNumber { get; set; } =1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public LearningTaskStatus? Status { get; set; }
    }
}