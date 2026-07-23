using System.Net;
using System.Text.Json;
using TraineeManagement.Api.DTOs;
using System.Net.Mime;

namespace TraineeManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                ex,
                "An unhandled exception occurred while processing the request.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                ErrorResponse response = new()
                {
                    Message = "An unexpected error occurred. Please try again."
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            }

        }

    }

}