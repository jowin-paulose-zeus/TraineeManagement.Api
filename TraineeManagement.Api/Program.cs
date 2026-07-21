using TraineeManagement.Api.Services;
using TraineeManagement.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TraineeManagement.Api.Configuration;
using TraineeManagement.Data.Configuration;
using TraineeManagement.Api.Interfaces;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using TraineeManagement.API.Middleware;
using StackExchange.Redis;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/api-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<ILearningTaskService, LearningTaskService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITaskAssignmentService, TaskAssignmentService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();
builder.Services.AddScoped<IProcessingJobService, ProcessingJobService>();
builder.Services.AddControllers();
builder.Services.AddDbContext<TraineeDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    options.UseMySQL(connectionString);
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

JwtSettings jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>() ?? throw new Exception("JWT Configuration is missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
builder.Services.AddSwaggerGen(options =>
{
    // Configure basic information for the OpenAPI documentation (optional)
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ASP.NET Core Web API",
        Version = "v1",
        Description = "ASP.NET Core Web API with JWT authentication. " +
        "Target Framework is .NET 10. " +
        "Swashbuckle.AspNetCore 10.1.7 is used."
    });

    // Add a Security Scheme (using a JWT Bearer token).
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "https://localhost:5143",
            "https://localhost:6379",
            "https://localhost:3306")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection("FileStorage"));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration.GetSection("Redis:ConnectionString").Value;

    options.InstanceName = "TraineeManagement:";
});
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ"));

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue; // Removes multipart limit
    options.ValueLengthLimit = int.MaxValue;          // Removes single form value limit
    options.MemoryBufferThreshold = int.MaxValue;     // Optional memory buffer bump
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // Removes total request size cap
});
WebApplication? app = builder.Build();
string? redisConnection = app.Configuration["Redis:ConnectionString"];

try
{
    if (redisConnection != null){
    ConnectionMultiplexer? connection = await ConnectionMultiplexer.ConnectAsync(redisConnection);
    
    if (connection.IsConnected)
    {
        app.Logger.LogInformation("Redis connected successfully.");
    }
    }
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Unable to connect to Redis.");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Generates /openapi/v1.json

    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("ReactPolicy");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
