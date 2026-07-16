using System.Diagnostics.CodeAnalysis;

namespace TraineeManagement.Api.Models
{
    public class SubmissionFile
    {
        public SubmissionFile(){}
        [SetsRequiredMembers]
        public SubmissionFile(Submission submission, string originalFileName, string storedFileName, string contentType, long size, string checksum)
        {
            SubmissionId = submission.Id;
            Submission = submission;
            OriginalFileName = originalFileName;
            StoredFileName = storedFileName;
            ContentType = contentType;
            Size = size;
            Checksum = checksum;
        }
        public int Id { get; set;}
        public int SubmissionId { get; set;}
        public required Submission Submission { get; set; }
        public required string OriginalFileName { get; set;}
        public required string StoredFileName { get; set; }
        public required string ContentType { get; set; }
        public long Size { get; set; }
        public required string Checksum { get; set; }
    }
}