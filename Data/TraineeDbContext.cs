using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Data
{
    public class TraineeDbContext(DbContextOptions<TraineeDbContext> options) : DbContext(options)
    {
        public DbSet<Trainee> Trainees { get; set; }
    }
}