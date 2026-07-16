using Org.BouncyCastle.Ocsp;

namespace TraineeManagement.Api.DTOs
{
    public class FileStorageResponse
    {
        public int Id { get; set; }
        public int SubmissionId { get; set; }
        public required string ContentType { get; set; }
        public required string OriginalFileName { get; set; }
        public long Size { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}