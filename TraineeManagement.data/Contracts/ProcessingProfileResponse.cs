namespace TraineeManagement.Data.Contracts
{
    public class ProcessingProfileResponse
    {
        public int SubmissionId { get; set; }

        public bool RequiresChecksumValidation { get; set; }

        public bool RequiresVirusScan { get; set; }

        public int MaxFileSizeMb { get; set; }

        public List<string> AllowedExtensions { get; set; } = [];
    }
}