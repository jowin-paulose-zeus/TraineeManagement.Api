namespace TraineeManagement.Data.Configuration
{
    public class FileStorageSettings
    {
        public string StorageRoot { get; set; } = string.Empty;
        public long MaxFileSize { get; set; }
        public required List<string> AllowedExtentions { get; set; } = [];
    }
}