using TraineeManagement.Api.DTOs;

namespace TraineeManagement.Api.DTOs
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set;}
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public IEnumerable<T> Data { get; set; } = [];
    }
}