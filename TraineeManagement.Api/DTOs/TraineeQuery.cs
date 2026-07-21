using System.Diagnostics.Contracts;
using TraineeManagement.Data.Enums;

namespace TraineeManagement.Api.DTOs
{
    public class TraineeQuery
    {
        public int PageNumber { get; set; } =1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public TraineeStatus? Status { get; set; }
    }
}