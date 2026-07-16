using TraineeManagement.Api.DTOs;
using Microsoft.Extensions.Options;
using TraineeManagement.Api.Configuration;
using TraineeManagement.Api.Interfaces;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TraineeManagement.Api.Services
{
    public class LocalFileStorageService(TraineeDbContext context, IOptions<FileStorageSettings> fileStorageSettings) : IFileStorageService
    {
        private readonly TraineeDbContext _context = context;
        private readonly string _storageRoot = InitializeStorage(fileStorageSettings.Value.StorageRoot);
        private readonly IOptions<FileStorageSettings> _fileStorageSettings = fileStorageSettings;

        private static string InitializeStorage(string storagePath)
        {
            var root = Path.Combine(Directory.GetCurrentDirectory(), storagePath);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            return root;
        }


        public async Task<string> SaveAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string storedFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(_storageRoot, storedFileName);
            using FileStream stream = new(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return storedFileName;
        }

        public async Task<Stream> OpenReadAsync(string fileName)
        {
            string filePath = Path.Combine(_storageRoot, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.");
            }
            return await Task.FromResult<Stream>(
            new FileStream(filePath, FileMode.Open, FileAccess.Read));
        }

        public async Task<bool> ExistsAsync(string fileName)
        {
            string filePath = Path.Combine(_storageRoot, fileName);
            return await Task.FromResult(File.Exists(filePath));
        }

        public async Task<DownloadSubmissionFileResponse> DownloadAsync(int id)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    subFile => subFile.Id == id) ?? throw new KeyNotFoundException($"Submission file {id} was not found.");
            Stream? stream = await this.OpenReadAsync(submissionFile.StoredFileName);

            return new DownloadSubmissionFileResponse
            {
                Stream = stream,
                ContentType = submissionFile.ContentType,
                FileName = submissionFile.OriginalFileName
            };
        }
        public async Task DeleteAsync(int id)
        {
            SubmissionFile? submissionFile = await _context.SubmissionFiles
                .FirstOrDefaultAsync(
                    subFile => subFile.Id == id) ?? throw new KeyNotFoundException($"Submission file {id} was not found.");
            string filePath = Path.Combine(_storageRoot, submissionFile.StoredFileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await Task.CompletedTask;

            _context.SubmissionFiles.Remove(submissionFile);

            await _context.SaveChangesAsync();

        }
    }
}