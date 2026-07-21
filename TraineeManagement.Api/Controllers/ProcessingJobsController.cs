using Microsoft.AspNetCore.Mvc;
using TraineeManagement.Api.DTOs;
using TraineeManagement.Api.Interfaces;

namespace TraineeManagement.Api.Controllers;

[ApiController]
[Route("api/processing-jobs")]
public class ProcessingJobsController(IProcessingJobService service, ILogger<ProcessingJobsController> logger) : ControllerBase
{
    private readonly IProcessingJobService _service = service;
    private readonly ILogger<ProcessingJobsController> _logger = logger;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            ProcessingJobResponse job = await _service.GetByIdAsync(id);

            return Ok(job);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}