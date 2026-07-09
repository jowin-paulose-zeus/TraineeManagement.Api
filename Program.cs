using TraineeManagement.Api.Services;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TraineeManagement.Api.Configuration;
using TraineeManagement.Api.Interfaces;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Keeps the .NET 9 OpenAPI generator
builder.Services.AddDbContext<TraineeDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
    options.UseMySQL(connectionString);
});
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

WebApplication? app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Generates /openapi/v1.json

    // FIX: Attach the visual Swagger user interface
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TraineeManagement API v1");
        options.RoutePrefix = "swagger"; // Sets the path to localhost:5119/swagger
    });
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
