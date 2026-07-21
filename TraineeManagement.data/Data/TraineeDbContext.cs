using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.Models;


namespace TraineeManagement.Data.Data
{
    public class TraineeDbContext(DbContextOptions<TraineeDbContext> options) : DbContext(options)
    {
        public DbSet<Trainee> Trainees { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                Role = Enums.UserRoles.Admin,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            });
            modelBuilder.Entity<ProcessingJob>()
                .HasOne(job => job.Submission)
                .WithMany(submission => submission.ProcessingJobs)
                .HasForeignKey(job => job.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        public DbSet<Mentor> Mentors { get; set; }
        public DbSet<LearningTask> LearningTasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<SubmissionFile> SubmissionFiles { get; set; }
        public DbSet<ProcessingJob> ProcessingJobs { get; set; }
    }
}