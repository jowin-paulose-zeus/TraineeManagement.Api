using TraineeManagement.Api.Services;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ITraineeService, TraineeService>();
builder.Services.AddDbContext<TraineeDbContext>(options =>
    options.UseInMemoryDatabase("TraineeDb"));
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // Keeps the .NET 9 OpenAPI generator

var app = builder.Build();


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
