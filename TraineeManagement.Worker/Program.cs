using TraineeManagement.Worker;
using TraineeManagement.Worker.Configuration;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.Data;
using TraineeManagement.Worker.Interfaces;
using TraineeManagement.Worker.Services;
using TraineeManagement.Data.Configuration;

HostApplicationBuilder? builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<ISubmissionProcessorService, SubmissionProcessorService>();
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddDbContext<TraineeDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    options.UseMySQL(connectionString);
});
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage"));

IHost? host = builder.Build();
host.Run();
