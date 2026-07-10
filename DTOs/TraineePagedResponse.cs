using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.DTOs
{
    public class TraineePagedResponse<TraineeResponseRequest>
    {
        public int PageNumber { get; set;}
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<TraineeResponseRequest> Data { get; set; } = [];
    }
}